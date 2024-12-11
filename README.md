# OpenLaunch

OpenLaunch is a self-hostable Blazor application, designed to manage pre-launch waitlists. It provides a centralized solution for collecting user signups, creating email templates, sending bulk emails, and tracking metrics such as signups over time and email deliverability.

OpenLaunch exposes subscribe and unsubscribe API endpoints and includes an interface from which you can view, manage, and communicate with your waitlist subscribers.

It currently only supports AWS SES, but more integrations are welcome, especially popular providers like MailGun and SendGrid

## Table of Contents

- [Features](#features)
- [Prerequisites](#prerequisites)
- [Installation](#installation)
- [Configuration](#configuration)
- [Running the Application](#running-the-application)
- [Contributing](#contributing)

## Features

- **Subscribe Endpoint:** Collect pre-launch signups with a protected API endpoint. Just point your launch page's signup form at the endpoint and include your generated API key in the header.


- **API Keys:** Create and manage API keys to secure your subscribe endpoint.


- **From address management:** Save pairs of from addresses and display names. Pair them with templates to maintain a consistent brand voice.


- **Email Campaigns:** Design and send bulk emails using simple templates and a rich text editor. 


- **Self-hosted Images:** Add images to your emails. You can use remote links or you can upload your image directly to your server. OpenLaunch will set the link for you and make the image publicly avaliable.


- **Unsubscribe and bounce management:** Automatically attach unsubscribe headers and verify unsubscribe requests to prevent spoofing. Hook up to your provider's bounce notifications to automatically unsubscribe hard bounces.


- **Metrics Dashboard:** Monitor signups, email deliverability, and other key metrics.

## Prerequisites

For deploying OpenLaunch in production, ensure you have the following installed on your server:

- **Docker**
- **Docker Compose:** 
- **Git** 

*Note: The .NET 9 SDK is necessary for development.*

## Configuration

1. **OpenLaunch uses environment variables for configuration.** An example file is provided.

       cp .env.example .env
2. Edit the `.env` or add them to the docker-compose

    ```
    # Default admin credentials
    ADMIN_USERNAME="your_admin_username" # Defaults to admin if not included
    ADMIN_PASSWORD="your_secure_password" # Defaults to password if not included
    
    # Current Options are "MOCK" or "AWS"
    USE_SERVICE="MOCK"
    
    # AWS credentials if using
    AWS_DEFAULT_REGION="your_aws_region"
    AWS_ACCESS_KEY_ID="your_aws_access_key_id"
    AWS_SECRET_ACCESS_KEY="your_aws_secret_access_key"
    
    # Key for signing unsubscribe tokens
    UNSUBSCRIBE_KEY="your_unsubscribe_key"
    ```

   > #### Note
   > As of right now, only AWS SES is supported for sending emails and retrieving metrics. MOCK is suitable for testing only.

## Running the Application

1. `docker-compose up --build -d`
2. Access the application at `http://localhost:8080` or at your configured domain.
3. Log in using the username and password you set in .env
4. Create an API key and add it to your signup form's request headers.
5. Start collecting signups!

***Caution***: There is currently no way to reset a forgotten password. If you forget your password (after changing it, for example), you will have to use a tool to delete the admin user from the SQLite database directly. Shut down the docker container and start it back up, at which point a new admin user with your configured credentials will be seeded when you spin up the app. You will not lose any other data. 

## Contributing

Contributions are welcome, particularly for integrations with other email providers.

Setting up the local environment is straightforward. 
1. clone the repository locally:
   ```
   git clone https://github.com/frockett/OpenLaunch.git
   ```
2. Switch to the code folder: 
   ```
   cd OpenLaunch
   ```
3. Create your feature or fix branch
   ```
   git checkout -b <feature/feature-branch-name>
   ```
4. Configure the required `.env` variables based on `.env.example`. You can use `MOCK` under  `USE_SERVICE`
   ```
   cp .env.example .env
   ```
5. Restore and build the project
   ```
   dotnet restore
   dotnet build
   ```



