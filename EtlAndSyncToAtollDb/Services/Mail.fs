namespace Etl

open System.Net
open System.Net.Mail
open Literals
open System.IO

module Mail =

    let sendEmail ()  = 
        let fromAddress = new MailAddress(MailFrom)
        
        let recipients = 
           MailsTo
           |>List.map(fun email -> new MailAddress(email)) 
           
        use client = new SmtpClient("smtp.btk.bg", 25)
        client.EnableSsl <- false
        client.Credentials <- new NetworkCredential(MailFrom, "your-password")

        let message = new MailMessage(fromAddress, recipients.[0])
        recipients.[1..] 
        |> List.iter (fun recipient -> message.To.Add(recipient))

        message.Subject <- MailSubject
        message.Body <- MailBody
        
        let attachment = new Attachment(
            Path.Combine(LogFilePath, logFileName))
        message.Attachments.Add(attachment)

        client.Send(message)
