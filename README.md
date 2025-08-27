# Linnked Send a meaningful message, with a Linnk

Linnked is an anonymous student matching + messaging app.
You can send someone an anonymous “Linnk” (a message), and if they accept, they can reply. If both people agree, their identities get revealed. The whole idea is to make Valentine’s Day (or any day) on campus more fun, safe, and meaningful.

# Tech Stack
Backend: C# / .NET

Database: (Postgres sql hosted on render)

Deployed on azure

Authentication is email-based (people sign up with emails only).

AI: OpenAI (for generating messages from prompts)

Email Delivery: Google SMTP service

# Features
Email signup 

Prompt-driven profiles → makes messages more personal

You can either type your message in yourself or ask AI to do that for you by giving a brief decription of who the person is

Then you can state your preference if you want it to be multi paged or single paged, the font and several other stuufff

You can the share the link or download the flash cardsss

Then teh receiver clicks on the link and sees the message and decides to accept or reject and you get their response in your email.

# Clone this repo
git clone https://github.com/hasbiyallah01/linnked.git

cd linnked

# Install dependencies
dotnet restore

# Set environment variables
Example:
Host=localhost;Port=5432;Database=linnked;Username=postgres;Password=yourpassword

# Update migration
dotnet ef database update

# Run the project
dotnet run

The app should running at:
http://localhost:7205/

# Why I Built This
I wanted to make something fun for students that isn’t just another random chat app. Most people are shy about expressing themselves, so Linnked gives them a way to do it anonymously, with privacy and consent built in.

# Struggles / Learnings
Designing the approval/reveal flow took a lot of iteration because when I completed the project I sent it to my class group to use it, they used it to an extent where my daily 500 email limit from SMTP got exhausted they people started thinking their crush dislike them because they weren't able to get an email response, I had to remove the first welcoome email which is attached below so each user just get email based on the number of link they had generated instead of a welcome email to everyone who comes onboard and had to track the calls to the service so I implemented key rotation to switch SMTP keys once a limit is reached..
Learned a ton about privacy-first design, user trust, and structuring social apps.
<img width="1338" height="719" alt="image" src="https://github.com/user-attachments/assets/ff123908-1594-493c-82a3-f1efb0d2d8bb" />

# Credits
Inspired by Valentine’s Day campus events and similar “matching” apps.
Built as a personal learning project and to my amazing friends and classmates that supported you're the bessstt

That’s it. It’s still in development, so expect things to break.
[![Athena Award Badge](https://img.shields.io/endpoint?url=https%3A%2F%2Faward.athena.hackclub.com%2Fapi%2Fbadge)](https://award.athena.hackclub.com?utm_source=readme)
