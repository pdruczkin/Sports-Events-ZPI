using Application.Common.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Common;

public static class Mails
{
    const string frontendUrl = "http://localhost:3000/";
    public static EmailDto GetForgotPasswordEmail(string email, string username, string token)
    {
        return new EmailDto
        {
            To = email,
            Subject = "ZPI - Odzyskaj hasło",
            Body = $$"""
            <!DOCTYPE html>
            <html lang="en">
            <head>
                <meta charset="UTF-8">
                <meta name="viewport" content="width=device-width, initial-scale=1.0">
                <title>Resetowanie hasła</title>
                <style>
                    body {
                        font-family: 'Arial', sans-serif;
                        background-color: #f4f4f4;
                        margin: 0;
                        padding: 0;
                        text-align: center;
                    }

                    .container {
                        max-width: 600px;
                        background-color: #ffffff;
                        padding: 20px;
                        border-radius: 10px;
                        box-shadow: 0 0 10px rgba(0, 0, 0, 0.1);
                    }

                    h1 {
                        color: #333333;
                    }

                    p {
                        color: #555555;
                    }

                    .button {
                        display: inline-block;
                        padding: 10px 20px;
                        background-color: #2196f3;
                        color: #ffffff;
                        text-decoration: none;
                        border-radius: 5px;
                        margin-top: 20px;
                    }

                    a {
                        color: #ffffff !important;
                    }
                </style>
            </head>
            <body>
                <div class="container">
                    <h1>Resetowanie hasła</h1>
                    <p>Witaj użytkowniku {{username}}!</p>
                    <p>Otrzymujesz tę wiadomość, ponieważ poprosiłeś o zresetowanie hasła do swojego konta.</p>
                    <p>Kliknij poniższy przycisk, aby przejść do procesu resetowania hasła:</p>
                    <a href={{frontendUrl}}forgot-password?token={{token}} class="button">Zresetuj hasło</a>
                    <p>Jeśli to nie Ty prosiłeś o zresetowanie hasła, zignoruj tę wiadomość.</p>
                </div>
            </body>
            </html>
            
                 
            """
        };
    }

    public static EmailDto GetConfirmRegisterNotificationEmail(string email, string username, string token)
    {
        return new EmailDto
        {
            To = email,
            Subject = "ZPI - Aktywuj konto",
            Body = $$"""
            <!DOCTYPE html>
            <html lang="en">
            <head>
                <meta charset="UTF-8">
                <meta name="viewport" content="width=device-width, initial-scale=1.0">
                <title>Potwierdzenie konta</title>
                <style>
                    body {
                        font-family: 'Arial', sans-serif;
                        background-color: #f4f4f4;
                        margin: 0;
                        padding: 0;
                        text-align: center;
                    }

                    .container {
                        max-width: 600px;
                        background-color: #ffffff;
                        padding: 20px;
                        border-radius: 10px;
                        box-shadow: 0 0 10px rgba(0, 0, 0, 0.1);
                    }

                    h1 {
                        color: #333333;
                    }

                    p {
                        color: #555555;
                    }

                    .button {
                        display: inline-block;
                        padding: 10px 20px;
                        background-color: #2196f3;
                        color: #ffffff;
                        text-decoration: none;
                        border-radius: 5px;
                        margin-top: 20px;
                    }

                    a {
                        color: #2196f3;
                    }
                </style>
            </head>
            <body>
                <div class="container">
                    <h1>Potwierdzenie konta</h1>
                    <p>Witaj użytkowniku {{username}}!</p>
                    <p>Dziękujemy za rejestrację w naszej aplikacji. Aby aktywować swoje konto, kliknij poniższy przycisk:</p>
                    <a href={{frontendUrl}}confirmed-email?token={{token}} class="button">Potwierdź konto</a>
                    <p>Jeśli nie rejestrowałeś się w naszej aplikacji, zignoruj tę wiadomość.</p>
                </div>
            </body>
            </html>            
            """
        };
    }
    public static EmailDto GetFriendInvitationNotificationEmail(string email, string username, string inviterUsername)
{
        return new EmailDto
        {
            To = email,
            Subject = "ZPI - Zaproszenie do znajomych",
            Body = $$"""
            <!DOCTYPE html>
            <html lang="en">
            <head>
                <meta charset="UTF-8">
                <meta name="viewport" content="width=device-width, initial-scale=1.0">
                <title>Zaproszenie do znajomych</title>
                <style>
                    body {
                        font-family: 'Arial', sans-serif;
                        background-color: #f4f4f4;
                        margin: 0;
                        padding: 0;
                        text-align: center;
                    }

                    .container {
                        max-width: 600px;
                        background-color: #ffffff;
                        padding: 20px;
                        border-radius: 10px;
                        box-shadow: 0 0 10px rgba(0, 0, 0, 0.1);
                    }

                    h1 {
                        color: #333333;
                    }

                    p {
                        color: #555555;
                    }

                    .button {
                        display: inline-block;
                        padding: 10px 20px;
                        background-color: #4caf50;
                        color: #ffffff;
                        text-decoration: none;
                        border-radius: 5px;
                        margin-top: 20px;
                    }

                    a {
                        color: #4caf50; /* Zmieniony kolor linku */
                        text-decoration: none;
                    }

                    a:hover {
                        text-decoration: underline;
                    }
                </style>
            </head>
            <body>
                <div class="container">
                    <h1>Zaproszenie do znajomych</h1>
                    <p>Witaj użytkowniku {{username}}!</p>
                    <p>Użytkownik {{inviterUsername}} wysłał Ci zaproszenie do znajomych. Kliknij poniższy przycisk, aby zaakceptować zaproszenie:</p>
                    <a href={{frontendUrl}}notifications class="button">Zaakceptuj zaproszenie</a>
                    <p>Jeśli nie chcesz dołączyć do znajomych, zignoruj tę wiadomość.</p>
                </div>
            </body>
            </html>
            
                        
            """
        };
    }
    public static EmailDto GetAcceptedFriendInvitationNotificationEmail(string email, string username, string inviteeUsername)
{
        return new EmailDto
        {
            To = email,
            Subject = "ZPI - Zaakceptowane zaproszenie do znajomych",
            Body = $$"""
            <!DOCTYPE html>
            <html lang="en">
            <head>
                <meta charset="UTF-8">
                <meta name="viewport" content="width=device-width, initial-scale=1.0">
                <title>Zaproszenie zaakceptowane</title>
                <style>
                    body {
                        font-family: 'Arial', sans-serif;
                        background-color: #f4f4f4;
                        margin: 0;
                        padding: 0;
                        text-align: center;
                    }

                    .container {
                        max-width: 600px;
                        background-color: #ffffff;
                        padding: 20px;
                        border-radius: 10px;
                        box-shadow: 0 0 10px rgba(0, 0, 0, 0.1);
                    }

                    h1 {
                        color: #333333;
                    }

                    p {
                        color: #555555;
                        margin-bottom: 20px;
                    }

                    .button {
                        display: inline-block;
                        padding: 10px 20px;
                        background-color: #4caf50;
                        color: #ffffff;
                        text-decoration: none;
                        border-radius: 5px;
                        margin-top: 20px;
                    }

                    a {
                        color: #2196f3 !important;
                        text-decoration: none;
                    }

                    a:hover {
                        text-decoration: underline;
                    }
                </style>
            </head>
            <body>
                <div class="container">
                    <h1>Zaproszenie zaakceptowane</h1>
                    <p>Użytkownik {{inviteeUsername}} zaakceptował Twoje zaproszenie do znajomych!</p>
                    <p>Teraz możecie razem organizować grupowe aktywności sportowe. Kliknij poniższy przycisk, aby przejść do aplikacji:</p>
                    <a href={{frontendUrl}} class="button">Przejdź do aplikacji</a>
                    <p>Dziękujemy za korzystanie z naszej aplikacji!</p>
                </div>
            </body>
            </html>          
                       
            """
        };
    }
    public static EmailDto GetDeletedMeetingNotificationEmail(string email, string username, string meetingTitle)
{
        return new EmailDto
        {
            To = email,
            Subject = "ZPI - Usunięte spotkanie",
            Body = $$"""
            <!DOCTYPE html>
            <html lang="en">
            <head>
                <meta charset="UTF-8">
                <meta name="viewport" content="width=device-width, initial-scale=1.0">
                <title>Odwołane spotkanie</title>
                <style>
                    body {
                        font-family: 'Arial', sans-serif;
                        background-color: #f4f4f4;
                        margin: 0;
                        padding: 0;
                        text-align: center;
                    }

                    .container {
                        max-width: 600px;
                        background-color: #ffffff;
                        padding: 20px;
                        border-radius: 10px;
                        box-shadow: 0 0 10px rgba(0, 0, 0, 0.1);
                    }

                    h1 {
                        color: #ff6347; /* Kolor ciemnoczerwony */
                    }

                    p {
                        color: #555555;
                    }

                    .button {
                        display: inline-block;
                        padding: 10px 20px;
                        background-color: #4caf50;
                        color: #ffffff;
                        text-decoration: none;
                        border-radius: 5px;
                        margin-top: 20px;
                    }

                    a {
                        color: #4caf50 !important; /* Kolor zielony dla linków */
                    }
                </style>
            </head>
            <body>
                <div class="container">
                    <h1>Odwołane spotkanie</h1>
                    <p>Szanowny użytkowniku {{username}},</p>
                    <p>Informujemy, że spotkanie "{{meetingTitle}}", w którym uczestniczyłeś, zostało niestety odwołane. Przepraszamy za wszelkie niedogodności.</p>
                    <p>Zapraszamy do sprawdzenia innych dostępnych spotkań lub do organizowania własnych aktywności.</p>
                    <p>Dziękujemy za korzystanie z naszej aplikacji!</p>
                </div>
            </body>
            </html>
                        
            """
        };
    }
    public static EmailDto GetUpdatedMeetingNotificationEmail(string email, string username, string meetingTitle, Guid meetingId)
{
        return new EmailDto
        {
            To = email,
            Subject = "ZPI - Zmiana szczegółów spotkania",
            Body = $$"""
            <!DOCTYPE html>
            <html lang="en">
            <head>
                <meta charset="UTF-8">
                <meta name="viewport" content="width=device-width, initial-scale=1.0">
                <title>Zmiana danych spotkania</title>
                <style>
                    body {
                        font-family: 'Arial', sans-serif;
                        background-color: #f4f4f4;
                        margin: 0;
                        padding: 0;
                        text-align: center;
                    }

                    .container {
                        max-width: 600px;
                        background-color: #ffffff;
                        padding: 20px;
                        border-radius: 10px;
                        box-shadow: 0 0 10px rgba(0, 0, 0, 0.1);
                    }

                    h1 {
                        color: #333333;
                    }

                    p {
                        color: #555555;
                        margin-bottom: 20px;
                    }

                    .button {
                        display: inline-block;
                        padding: 10px 20px;
                        background-color: #2196f3;
                        color: #ffffff;
                        text-decoration: none;
                        border-radius: 5px;
                        margin-top: 20px;
                    }

            		a {
            			color: #ffffff !important;
            		}
                </style>
            </head>
            <body>
                <div class="container">
                    <h1>Zmiana danych spotkania</h1>
                    <p>Witaj użytkowniku {{username}}!</p>
                    <p>Chcieliśmy Cię poinformować, że dane dotyczące spotkania "{{meetingTitle}}, w którym uczestniczysz, uległy zmianie. Kliknij poniższy przycisk, aby sprawdzić nowe informacje:</p>
                    <a href={{frontendUrl}}events/{{meetingId}} class="button">Sprawdź zmiany</a>
                    <p>Jeśli masz pytania lub potrzebujesz dodatkowych informacji, skontaktuj się z organizatorem spotkania.</p>
                </div>
            </body>
            </html>
            """
        };
    }
    public static EmailDto GetMeetingInvitationNotificationEmail(string email, string username, string inviterUsername, string meetingTitle, Guid meetingId)
    {
        return new EmailDto
        {
            To = email,
            Subject = "ZPI - Zaproszenie do spotkania",
            Body = $$"""
            <!DOCTYPE html>
            <html lang="en">
            <head>
                <meta charset="UTF-8">
                <meta name="viewport" content="width=device-width, initial-scale=1.0">
                <title>Informacja o otrzymaniu zaproszenia do spotkania</title>
                <style>
                    body {
                        font-family: 'Arial', sans-serif;
                        background-color: #f4f4f4;
                        margin: 0;
                        padding: 0;
                        text-align: center;
                    }

                    .container {
                        max-width: 600px;
                        background-color: #ffffff;
                        padding: 20px;
                        border-radius: 10px;
                        box-shadow: 0 0 10px rgba(0, 0, 0, 0.1);
                    }

                    h1 {
                        color: #333333;
                    }

                    p {
                        color: #555555;
                    }

                    .button {
                        display: inline-block;
                        padding: 10px 20px;
                        background-color: #4caf50;
                        color: #ffffff;
                        text-decoration: none;
                        border-radius: 5px;
                        margin-top: 20px;
                    }

            		a {
            			color: white !important
            		}
                </style>
            </head>
            <body>
                <div class="container">
                    <h1>Informacja o otrzymaniu zaproszenia do spotkania</h1>
                    <p>Witaj użytkowniku {{username}}!</p>
                    <p>Otrzymałeś zaproszenie do udziału w spotkaniu "{{meetingTitle}} od użytkownika {{inviterUsername}}. Kliknij poniższy przycisk, aby przejść do szczegółów spotkania:</p>
                    <a href={{frontendUrl}}events/{{meetingId}} class="button">Zobacz szczegóły spotkania</a>
                    <p>Jeśli nie chcesz uczestniczyć w tym spotkaniu, zignoruj tę wiadomość.</p>
                </div>
            </body>
            </html>
            
            
                        
            """
        };
    }
    public static EmailDto GetRemovedFromMeetingNotificationEmail(string email, string username, string removerUsername, string meetingTitle)
    {
        return new EmailDto
        {
            To = email,
            Subject = "ZPI - Usunięcie ze spotkania",
            Body = $$"""
            <!DOCTYPE html>
            <html lang="en">
            <head>
                <meta charset="UTF-8">
                <meta name="viewport" content="width=device-width, initial-scale=1.0">
                <title>Usunięcie ze spotkania</title>
                <style>
                    body {
                        font-family: 'Arial', sans-serif;
                        background-color: #f4f4f4;
                        margin: 0;
                        padding: 0;
                        text-align: center;
                    }

                    .container {
                        max-width: 600px;
                        background-color: #ffffff;
                        padding: 20px;
                        border-radius: 10px;
                        box-shadow: 0 0 10px rgba(0, 0, 0, 0.1);
                    }

                    h1 {
                        color: #333333;
                    }

                    p {
                        color: #555555;
                    }

                    .button {
                        display: inline-block;
                        padding: 10px 20px;
                        background-color: #d9534f; /* Kolor przycisku czerwony */
                        color: #ffffff;
                        text-decoration: none;
                        border-radius: 5px;
                        margin-top: 20px;
                    }

                    a {
                        color: white !important;
                    }
                </style>
            </head>
            <body>
                <div class="container">
                    <h1>Usunięcie ze spotkania</h1>
                    <p>Witaj użytkowniku {{username}}!</p>
                    <p>Z przykrością informujemy, że zostałeś usunięty ze spotkania "{{meetingTitle}} przez użytkownika {{removerUsername}}. Jeśli masz pytania, skontaktuj się z organizatorem spotkania.</p>
                    <p>Jeśli uważasz, że to nieprawidłowe, skontaktuj się z administratorem aplikacji.</p>
                    <a href={{frontendUrl}} class="button">Przejdź do aplikacji</a>
                </div>
            </body>
            </html>
            
            """
        };
    }




}
