# LeBonCoinAlert

LeBonCoinAlert is a .NET application designed to monitor and alert users about new listings on LeBonCoin. This project
uses Docker for containerization and deployment.

## Prerequisites

Docker or .NET SDK 8.0

## Getting Started

### Building and Running with Docker

1. **Clone the repository**

2. **Create a `.env.docker-compose` file :**
    ```sh
    cp .env.docker-compose.example .env.docker-compose
    ```
3. **Create a new bot or get the token of an existing one (see instructions below) and save them in the
   .env.docker-compose file**

4. **Build and run the Docker container:**
    ```sh
    docker compose up -d
    ```

### Environment Variables

The application uses environment variables defined in the `.env.docker-compose` file. Ensure this file contains the
necessary configuration:

```dotenv
TOKEN=your_token_here
```

## Telegram Bot Setup

To receive notifications via Telegram, you need to set up a Telegram bot and obtain your chat ID. Follow these steps:

### Create a Telegram Bot

1. Open the chat with `@BotFather`.
2. Enter `/newbot`.
3. Enter the name of your bot (e.g., `LeBonCoinAlert Bot`).
4. Enter a unique username for your bot (e.g., `my_leboncoin_alert_bot`).
5. Copy the token provided by `@BotFather`.

### Use the bot

1. Open the chat with your newly created bot.
2. Enter `/start`, now you can add urls to monitor.
3. Enter `/watch <url>` to start monitoring a new listing.

## License

This project is licensed under the MIT License. See the `LICENSE` file for more details.