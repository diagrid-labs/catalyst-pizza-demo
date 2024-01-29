# Pizza Workflow Demo

![catalyst pizza workflow](/images/catalyst-pizza-workflow.gif)

This repository contains a solution that demonstrates how to use the serverless Dapr APIs for workflow, service invocation, pub/sub, and state management to build a distributed pizza ordering system.

The solution includes:

- [Vercel](https://vercel.com); to host the website (based on Vue) and two serverless functions (JavaScript).
- Two [Dapr](http://dapr.io) web services written in .NET, *PizzaOrderService* and *KitchenService*.
- [Ably](https://ably.com/); to provide realtime communication between the *PizzaOrderService* and the website.
- A key/value store to manage inventory (managed by Diagrid).
- A pub/sub message broker to communicate between the *PizzaOrderService* and the *KitchenService* (managed by Diagrid).
- [Diagrid Catalyst](https://www.diagrid.io/catalyst); the serverless glue between services and resources powered by Dapr.

![Pizza Store Architecture](/images/pizza-store-architecture-v2.png)

The *PizzOrderService* contains a Dapr workflow that orchestrates the activities for checking the inventory and communicating with the *KitchenService*.

![Workflow](/images/pizza-store-workflow-v1.png)

> *The workflow also contains activities for sending realtime messages to the website, these have been omitted from the workflow diagram to keep it concise.*

This repository contains two variations:

1. The [`local-dapr` branch](https://github.com/diagrid-labs/catalyst-pizza-demo/tree/local-dapr) uses Dapr in [self-hosted mode](https://docs.dapr.io/operations/hosting/self-hosted/self-hosted-overview/) using the Dapr CLI with multi-app-run for the two .NET services. It uses a local Redis instance for state management, pub/sub, and workflow.
2. The ['main' branch](https://github.com/diagrid-labs/catalyst-pizza-demo/tree/main) uses a managed version of the Dapr API provided by [Diagrid Catalyst](https://www.diagrid.io/catalyst). The Diagrid CLI is used to run the .NET services. It also uses the Diagrid managed key/value store (for state management and workflow), and pub/sub message broker.

## Running the Diagrid Catalyst variation locally

### Prerequisites

- [Diagrid Catalyst account](https://catalyst.diagrid.io/) and the [Diagrid CLI](https://docs.dapr.io/getting-started/install-dapr-cli/)
- [Vercel account (hobby)](https://vercel.com/signup) and the [Vercel CLI](https://vercel.com/docs/cli)
- [Ably account (free)](https://www.ably.com/signup)
- [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)
- [Node 18](https://nodejs.org/en/download/)

> This repository contains a devcontainer configuration that has the following preinstalled:
> - .NET 8
> - Node LTS
> - Vercel CLI
> - Diagrid CLI

> You can use this devcontainer [locally in VSCode](https://code.visualstudio.com/docs/devcontainers/containers) (requires [Docker Desktop](https://www.docker.com/products/docker-desktop/)) or directly in [GitHub Codespaces](https://github.com/features/codespaces). The `npm install` and `dotnet build` commands described in this README can be skipped if the devcontainer is used.

### Setup

1. [Fork](https://github.com/diagrid-labs/catalyst-pizza-demo/fork) this repo and clone it locally.

#### Ably

1. Log into the [Ably portal](https://ably.com/accounts/).
1. Using the Ably portal: copy the [Root API key](https://ably.com/docs/ids-and-keys#api-key) from the default Ably app.

#### Vercel

1. Open a terminal in the root of the repository login with the Vercel CLI:

   ```bash
   vercel login
   ```

2. Go to the `front-end` folder and run:

   ```bash
   npm install
   ```

3. Go back to the root of the repository and setup the Vercel project by running:

   ```bash
   vercel
   ```

    Follow the CLI prompts, and select the following options:
    - Setup and deploy: `Y`
    - Scope: `<account name>`
    - Link to existing project: `N`
    - What's your project's name? `catalyst-pizza-project`
    - In which directory is your code located? `./front-end`
    - Want to modify these settings? [y/N] `y`
    - Build Command: `vite build`
    - Development Command: `vite`
    - Output Directory: `dist`
    - Wait for the depployment to complete.

1. An environment variable is used in the *getAblyToken* function to generate a token for the website to communicate with the Ably realtime service. Add the *Ably API token* variable by running `vercel env add`:
    - Variable name: `ABLY_API_KEY`
    - Variable value: *Use the Ably API key obtained from the Ably portal earlier*
    - Select `Development` as the environment.
2. Another environment variable is used in the *placeOrder* function to send a request to the *PizzaOrderService*. Add the *WORKFLOW_URL* variable by running `vercel env add`:
    - Variable name: `WORKFLOW_URL`
    - Variable value: `http://localhost:5064/workflow/orderReceived`
    - Select `Development` as the environment.
3. Run `vercel env pull` to pull the environment variables from Vercel to your local environment.
4. Run `vercel build` to build the website and the serverless functions.

### Catalyst

1. Open another terminal in the root of the repository and use the Diagrid CLI to login to Diagrid:

   ```bash
   diagrid login
   ```

1. Create a new Catalyst project named `catalyst-pizza-project` and use the Diagrid managed PubSub broker & KV store, and enable the managed workflow API:

    ```bash
    diagrid project create catalyst-pizza-project --deploy-managed-pubsub --deploy-managed-kv --enable-managed-workflow --wait
    ```

1. To set this project as the default in the CLI run:

    ```bash
    diagrid project use catalyst-pizza-project
    ```

1. Create a new App ID for the *PizzaOrderService*:

    ```bash
    diagrid appid create pizzaorderservice
    ```

1. Create a new App ID for the *KitchenService*:

    ```bash
    diagrid appid create kitchenservice
    ```

1. before continuing check the App IDs to make sure they have been created:

	```bash
	diagrid appid list
	```

1. Create a pub/sub subscription for the *KitchenService* to receive messages from the *PizzaOrderService*:

	```bash
	diagrid subscription create pizzaorderssub --connection pubsub --topic pizza-orders --route /prepare --scopes kitchenservice
	```

1. Create a pub/sub subscription for the *PizzaOrderService* to receive messages from the *KitchenService*:

	```bash
	diagrid subscription create preparedorderssub --connection pubsub --topic prepared-orders --route /workflow/orderPrepared --scopes pizzaorderservice
	```

1. Verify that creation of the subscriptions is completed:

	```bash
	diagrid subscription list
	```

1. Run `diagrid dev scaffold` to create a new local dev environment file . This creates a yaml file, named *dev-\<PROJECT NAME\>.yaml* with the following content:

	```yaml
	project: catalyst-pizza-project
	apps:
	- appId: kitchenservice
	appPort: 0
	env:
		DAPR_API_TOKEN: diagrid://<dapr_api_token>
		DAPR_APP_ID: kitchenservice
		DAPR_GRPC_ENDPOINT: https://<grpc_endpoint>
		DAPR_HTTP_ENDPOINT: https://<http_endpoint>
	workDir: kitchenservice
	command: []
	- appId: pizzaorderservice
	appPort: 0
	env:
		DAPR_API_TOKEN: diagrid://<dapr_api_token>
		DAPR_APP_ID: pizzaorderservice
		DAPR_GRPC_ENDPOINT: https://<grpc_endpoint>
		DAPR_HTTP_ENDPOINT: https://<http_endpoint>
	workDir: pizzaorderservice
	command: []
	appLogDestination: ""
	```

1. Update the `appPort` for the *kitchenservice* to `5066`
1. Update the `appPort` for the *pizzaorderservice* to `5064`.
1. Update the `command` arguments to `["dotnet", "run"]` for both apps.
1. Update the `workDir` argument to point to `back-end/KitchenService` and `back-end/PizzaOrderService` respectively.
1. Update the `appLogDestination` to `console`.
1. Add an `ABLY_API_KEY` environment variable for the *pizzaorderservice* app and set the value to the Ably API key obtained from the Ably portal.
1. Save the changes to the file.

### Running the solution

1. Open a terminal in the root of the repository.
1. To restore and build the dotnet project run:

   ```bash
   dotnet build ./back-end/PizzaOrderService
   dotnet build ./back-end/KitchenService
   ```

1. Run `diagrid dev start` to start the `PizzaOrderService` and the `KitchenService`.
1. Back in the root of the repository run `vercel dev` to start the website and the serverless functions locally.
1. Navigate to the URL provided by the Vercel CLI to view the website.
1. Select some pizzas, place an order, and watch the progress of the workflow in realtime.

![tmnt](/images/tmnt.gif)

## More information

Any questions or comments about this sample? Join the [Diagrid Community on Discourse](https://community.diagrid.io/invites/fAUrdyBbie) and post a message the *Catalyst* category.
Have you made something with Catalyst? Post a message in the *Built with Catalyst* category, we love to see your creations!
