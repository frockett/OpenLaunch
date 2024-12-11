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
- [Usage](#usage-and-api)
- [Contributing](#contributing)
- [Screenshots](#screenshots)

## Features

- **Subscribe Endpoint:** Collect pre-launch signups with a protected API endpoint. Just point your launch page's signup form at the endpoint and include your generated API key in the header.


- **API Keys:** Create and manage API keys to secure your subscribe endpoint.


- **From address management:** Save pairs of from addresses and display names. Pair them with templates to maintain a consistent brand voice.


- **Email Campaigns:** Design and send bulk emails using simple templates and a rich text editor. 


- **Self-hosted Images:** Add images to your emails. You can use remote links, or you can upload your image directly to your server. OpenLaunch will set the link for you and make the image publicly avaliable.


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

## Usage and API

### External Endpoints

#### Subscribe Endpoint
**POST** `/api/subscribe`

This endpoint allows users to subscribe to your waitlist by providing their email address and consent preferences.

#### Request Headers
- `x-api-key` (string, required): Your API key for authentication.

#### Request Body
The request should be a JSON object with the following properties:

| Field          | Type    | Required | Description                                                                                                                              |
|----------------|---------|----------|------------------------------------------------------------------------------------------------------------------------------------------|
| `Email`        | string  | Yes      | The email address of the subscriber.                                                                                                     |
| `BetaConsent`  | boolean | No       | Indicates if the user consents to beta updates.                                                                                          |
| `UpdateConsent`| boolean | Yes      | Indicates if the user consents to updates about the product. Non-consenting users do not appear in the subscriber email list by default. |

**Example Request**:
```json
{
  "Email": "user@example.com",
  "BetaConsent": true,
  "UpdateConsent": true
}
```

#### Health Check Endpoint

**GET** `/api/health`

This endpoint provides a simple health check for the application. It is intended to be used by monitoring systems or load balancers to verify that the application is running and responsive.

**Request**:
- No authentication or parameters are required.

**Response**:
- **200 OK**: The application is healthy and operational.
- **503 Service Unavailable**: The application is unhealthy or not operational.


### Internal Endpoints

#### Unsubscribe Endpoint
**POST** `/api/unsubscribe`

This endpoint is used to unsubscribe a user from receiving further emails. It is triggered by users clicking the unsubscribe link in emails sent by the application. The unsubscribe link includes the required query parameters (`Email` and `Token`).

**Request Parameters (Query)**:

| Parameter  | Type   | Required | Description                                          |
|------------|--------|----------|------------------------------------------------------|
| `Email`    | string | Yes      | The email address of the subscriber to be removed.  |
| `Token`    | string | Yes      | A secure token unique to the email, required for validation. |

**Security**:
- The token is auto-generated and attached to the unsubscribe URL in the headers of every email.
- The token is created and later validated by generating a hash using your configured `UNSUBSCRIBE_KEY` and the user's e-mail.
- This endpoint is **anonymous**, as authentication is not required to unsubscribe.
- All unsubscribe requests return a `200 OK` response, but errors are logged. This may change in the near future.

**Example Request**:
POST `/api/unsubscribe?Email=user@example.com&Token=securetoken123`

#### Bounce Endpoint
**POST** `/sns/bounce`

This endpoint is used to handle bounce notifications from email providers. As of writing, it only support notifications via AWS SNS, but support for SendGrid and MailGun webhooks are planned. 

It is intended for internal use and is not meant to be called directly by users. The endpoint processes notifications to track delivery issues and maintain email hygiene.

**Request Body**:
The request body contains the bounce notification payload provided by the email provider in JSON format. The structure of the payload varies depending on the provider.

**Example Request**:
```json
{
  "Type": "Notification",
  "MessageId": "example-message-id",
  "TopicArn": "example-topic-arn",
  "Message": "{...}", // Email provider-specific bounce details
  "Timestamp": "2023-01-01T00:00:00.000Z",
  "SignatureVersion": "1",
  "Signature": "example-signature",
  "SigningCertURL": "example-cert-url",
  "UnsubscribeURL": "example-unsubscribe-url"
}
```

## Contributing

Contributions are welcome, particularly for integrations with other email providers.

Setting up the local environment is straightforward. 
1. fork and then clone the forked repository locally:
   ```
   git clone https://github.com/<your github username>/OpenLaunch.git
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



