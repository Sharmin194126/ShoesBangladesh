using MailKit.Net.Smtp;
using MailKit.Security;
using MimeKit;
using ShoesBangladeshWebApp.Models;

namespace ShoesBangladeshWebApp.Services
{
    public class EmailService
    {
        private readonly IConfiguration _config;

        public EmailService(IConfiguration config)
        {
            _config = config;
        }

        public async Task SendOrderReceiptEmailAsync(OrderReceiptViewModel receipt)
        {
            var smtpHost = _config["EmailSettings:SmtpHost"] ?? "smtp.gmail.com";
            var smtpPort = int.Parse(_config["EmailSettings:SmtpPort"] ?? "587");
            var smtpUser = _config["EmailSettings:SmtpUser"] ?? "";
            var smtpPass = _config["EmailSettings:SmtpPass"] ?? "";
            var senderName = _config["EmailSettings:SenderName"] ?? "Shoes Bangladesh";
            
            var adminEmail = _config["EmailSettings:AdminEmail"];
            if (string.IsNullOrEmpty(adminEmail) || adminEmail == "admin@shoesbangladesh.com")
            {
                adminEmail = _config["CompanySettings:Email"] ?? smtpUser;
            }

            var htmlBody = BuildReceiptHtml(receipt);
            var plainTextBody = BuildReceiptPlainText(receipt);

            using var client = new SmtpClient();
            try
            {
                await client.ConnectAsync(smtpHost, smtpPort, SecureSocketOptions.StartTls);
                await client.AuthenticateAsync(smtpUser, smtpPass);

                // 1. Send to Customer
                if (!string.IsNullOrEmpty(receipt.CustomerEmail))
                {
                    var customerMsg = new MimeMessage();
                    customerMsg.From.Add(new MailboxAddress(senderName, smtpUser));
                    customerMsg.To.Add(new MailboxAddress(receipt.CustomerName, receipt.CustomerEmail));
                    customerMsg.Subject = $"Order Confirmation - #{receipt.InvoiceNumber}";
                    
                    var builder = new BodyBuilder { 
                        HtmlBody = htmlBody,
                        TextBody = plainTextBody 
                    };
                    customerMsg.Body = builder.ToMessageBody();
                    
                    await client.SendAsync(customerMsg);
                }

                // 2. Send to Admin
                var adminMsg = new MimeMessage();
                adminMsg.From.Add(new MailboxAddress(senderName, smtpUser));
                adminMsg.To.Add(new MailboxAddress("Admin", adminEmail));
                adminMsg.Subject = $"New Order Alert - Invoice #{receipt.InvoiceNumber}";
                
                var adminBuilder = new BodyBuilder { 
                    HtmlBody = htmlBody,
                    TextBody = plainTextBody 
                };
                adminMsg.Body = adminBuilder.ToMessageBody();
                
                await client.SendAsync(adminMsg);

                await client.DisconnectAsync(true);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[EMAIL ERROR] {ex.Message}");
                if (ex.InnerException != null) Console.WriteLine($"[INNER ERROR] {ex.InnerException.Message}");
                throw;
            }
        }

        private string BuildReceiptPlainText(OrderReceiptViewModel r)
        {
            var itemsText = "";
            foreach (var item in r.Cart.Items)
            {
                itemsText += $"- {item.ProductName} (Size: {item.Size}) x {item.Quantity}: ৳{item.LineTotal.ToString("#,##0")}\n";
            }

            return $@"
Order Invoice #{r.InvoiceNumber}
Shoes Bangladesh - Your Trusted Footwear Partner

Customer: {r.CustomerName}
Email: {r.CustomerEmail}
Order Date: {r.OrderDate:dd MMMM yyyy, hh:mm tt}

Ordered Items:
{itemsText}
Subtotal: ৳{r.Cart.SubTotal.ToString("#,##0")}
VAT: ৳{r.Cart.TotalVat.ToString("#,##0")}
Grand Total: ৳{r.Cart.GrandTotal.ToString("#,##0")}

Delivery Address:
{r.Address}, {r.City}, {r.ZipCode}

Thank you for your order!
Shoes Bangladesh
";
        }

        private string BuildReceiptHtml(OrderReceiptViewModel r)
        {
            var itemsHtml = "";
            foreach (var item in r.Cart.Items)
            {
                itemsHtml += $@"
                <tr>
                    <td style='padding:10px; border-bottom:1px solid #eee;'>{item.ProductName}<br><small style='color:#888'>Size: {item.Size} | Qty: {item.Quantity}</small></td>
                    <td style='padding:10px; border-bottom:1px solid #eee; text-align:right;'>৳{item.UnitPrice.ToString("#,##0")}</td>
                    <td style='padding:10px; border-bottom:1px solid #eee; text-align:right;'>{item.Quantity}</td>
                    <td style='padding:10px; border-bottom:1px solid #eee; text-align:right; font-weight:bold;'>৳{item.LineTotal.ToString("#,##0")}</td>
                </tr>";
            }

            var vatRow = r.Cart.TotalVat > 0
                ? $"<tr><td colspan='3' style='text-align:right; padding:8px; color:#555;'>Total VAT:</td><td style='text-align:right; padding:8px;'>৳{r.Cart.TotalVat.ToString("#,##0")}</td></tr>"
                : "";

            return $@"
<!DOCTYPE html>
<html>
<head><meta charset='utf-8'><style>
    body {{ font-family: 'Segoe UI', sans-serif; color: #333; background: #f5f5f5; margin:0; padding:20px; }}
    .invoice-box {{ max-width:700px; margin:auto; background:#fff; border-radius:12px; overflow:hidden; box-shadow:0 4px 20px rgba(0,0,0,0.1); }}
    .header {{ background: linear-gradient(135deg, #D00000, #8B0000); color:#fff; padding:30px 40px; }}
    .header h1 {{ margin:0; font-size:28px; letter-spacing:-1px; }}
    .header p {{ margin:5px 0 0; opacity:0.85; font-size:13px; }}
    .body {{ padding:30px 40px; }}
    .badge-invoice {{ background:#fff; color:#D00000; font-weight:800; font-size:1.1rem; padding:6px 18px; border-radius:30px; display:inline-block; margin-top:10px; }}
    .info-grid {{ display:grid; grid-template-columns:1fr 1fr; gap:20px; margin:25px 0; }}
    .info-block {{ background:#f9f9f9; padding:15px; border-radius:8px; }}
    .info-block strong {{ display:block; color:#888; font-size:11px; text-transform:uppercase; letter-spacing:0.5px; margin-bottom:5px; }}
    table {{ width:100%; border-collapse:collapse; margin-top:20px; }}
    thead th {{ background:#1a1a1a; color:#fff; padding:12px 10px; text-align:left; font-size:13px; }}
    thead th:not(:first-child) {{ text-align:right; }}
    .total-row {{ background:#f9f9f9; font-weight:bold; }}
    .grand-total {{ background:#D00000; color:#fff; font-size:1.1rem; }}
    .footer {{ background:#f5f5f5; text-align:center; padding:20px; color:#aaa; font-size:12px; border-top:1px solid #eee; }}
</style></head>
<body>
<div class='invoice-box'>
    <div class='header'>
        <h1>🥾 Shoes Bangladesh</h1>
        <p>Premium Footwear — Order Invoice</p>
        <div class='badge-invoice'>Invoice #{r.InvoiceNumber}</div>
    </div>
    <div class='body'>
        <div class='info-grid'>
            <div class='info-block'>
                <strong>Bill To</strong>
                {r.CustomerName}<br>
                {r.CustomerEmail}<br>
                {r.Phone}
            </div>
            <div class='info-block'>
                <strong>Delivery Address</strong>
                {r.Address}<br>
                {r.City}, {r.ZipCode}
            </div>
            <div class='info-block'>
                <strong>Order Date</strong>
                {r.OrderDate:dd MMMM yyyy, hh:mm tt}
            </div>
            <div class='info-block'>
                <strong>Payment Method</strong>
                {r.PaymentMethod}
            </div>
        </div>

        <table>
            <thead>
                <tr>
                    <th>Product</th>
                    <th style='text-align:right;'>Unit Price</th>
                    <th style='text-align:right;'>Qty</th>
                    <th style='text-align:right;'>Total</th>
                </tr>
            </thead>
            <tbody>
                {itemsHtml}
                <tr class='total-row'>
                    <td colspan='3' style='text-align:right; padding:10px;'>Subtotal:</td>
                    <td style='text-align:right; padding:10px;'>৳{r.Cart.SubTotal.ToString("#,##0")}</td>
                </tr>
                {vatRow}
                <tr class='grand-total'>
                    <td colspan='3' style='text-align:right; padding:12px; font-weight:bold;'>Grand Total:</td>
                    <td style='text-align:right; padding:12px; font-weight:bold;'>৳{r.Cart.GrandTotal.ToString("#,##0")}</td>
                </tr>
            </tbody>
        </table>

        <p style='margin-top:25px; color:#555; font-size:13px;'>Thank you for your order! We will process your delivery as soon as possible.</p>
    </div>
    <div class='footer'>© {DateTime.Now.Year} Shoes Bangladesh. All rights reserved.</div>
</div>
</body></html>";
        }
    }
}
