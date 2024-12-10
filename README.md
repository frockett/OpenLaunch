# OpenLaunch

OpenLaunch is a Blazor application built with .NET 9, designed to manage pre-launch waitlists. It provides a centralized solution for collecting user signups, creating email templates, sending bulk emails, and tracking metrics such as signups over time and email deliverability.

It currently only supports AWS SES, but more integrations are welcome.

## Table of Contents

- [Features](#features)
- [Prerequisites](#prerequisites)
- [Installation](#installation)
- [Configuration](#configuration)
- [Running the Application](#running-the-application)
- [Contributing](#contributing)

## Features

- **Secure Subscribe Endpoint:** Collect pre-launch signups with a protected API endpoint.
- **API Key Management:** Create and manage API keys to secure your subscribe endpoint.
- **Email Campaigns:** Design and send bulk emails using simple templates and a rich text editor. Supports remote images and hosting uploaded images on your domain.
- **Unsubscribe and bounce management:** Automatically attach unsubscribe headers and verify unsubscribe requests to prevent spoofing. Hook up to your provider's bounce notifications to automatically unsubscribe hard bounces.
- **Metrics Dashboard:** Monitor signups, email deliverability, and other key metrics.

## Prerequisites

For deploying OpenLaunch in production, ensure you have the following installed on your server:

- **Docker**
- **Docker Compose:** 
- **Git** 

*Note: The .NET 9 SDK is **not** required for deploying with Docker, but is necessary for development.*

## Configuration

1. **OpenLaunch uses environment variables for configuration.** An example file is provided.

       ```cp .env.example .env```
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
2. Access the application at `http://localhost:8080` if deployed locally.
3. ***Caution***: it is suggested you change your password after successfully logging in the first time.

## Contributing

Contributions are welcome, particularly for integrations with other email providers.


