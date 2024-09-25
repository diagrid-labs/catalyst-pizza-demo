# Pizza Workflow Demo

![catalyst pizza workflow](/images/catalyst-pizza-workflow.gif)

This repository contains a solution that demonstrates how to use the Diagrid Catalyst serverless Dapr APIs for workflow, service invocation, pub/sub, and state management to build a distributed pizza ordering system.

> 📖 Read the [blog post](https://www.diagrid.io/blog/build-a-distributed-pizza-store-in-net-with-serverless-dapr-apis) for more context.

The solution includes:

- [Vercel](https://vercel.com); to host the website (based on Vue) and two serverless functions (JavaScript).
- Two [Dapr](http://dapr.io) web services written in .NET, *PizzaOrderService* and *KitchenService*.
- [Ably](https://ably.com/); to provide realtime communication between the *PizzaOrderService* and the website.
- A key/value store to manage inventory (managed by Diagrid).
- A pub/sub message broker to communicate between the *PizzaOrderService* and the *KitchenService* (managed by Diagrid).
- [Diagrid Catalyst](https://www.diagrid.io/catalyst) that offers serverless APIs for communication, state, and workflow powered by Dapr.

![Pizza Store Architecture](/images/pizza-store-architecture-v2.png)

The *PizzaOrderService* contains a Dapr workflow that orchestrates the activities for checking the inventory and communicating with the *KitchenService*.

![Workflow](/images/pizza-store-workflow-v1.png)

> *The workflow also contains activities for sending realtime messages to the website, these have been omitted from the workflow diagram to keep it concise.*

## Running the Diagrid Catalyst variation locally

### Prerequisites

The following services, tools, and frameworks are required for this demo:

- [Diagrid Catalyst](https://www.diagrid.io/catalyst) account ([sign up](https://pages.diagrid.io/catalyst-early-access-waitlist) for private beta access) and the [Diagrid CLI](https://docs.diagrid.io/catalyst/references/cli-reference/intro)
- [Vercel account (hobby)](https://vercel.com/signup) and the [Vercel CLI](https://vercel.com/docs/cli)
- [Ably account (free)](https://www.ably.com/signup)
- [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)
- [Node 18](https://nodejs.org/en/download/)

> All the code for the website, serverless functions and back-end services is available in this [catalyst-pizza-demo](https://github.com/diagrid-labs/catalyst-pizza-demo) GitHub repository. The repository also contains a devcontainer configuration that has the following preinstalled: .NET 8, Node LTS, Vercel CLI, Diagrid CLI.


> You can use this devcontainer [locally in VSCode](https://code.visualstudio.com/docs/devcontainers/containers) (requires [Docker Desktop](https://www.docker.com/products/docker-desktop/)) or directly in [GitHub Codespaces](https://github.com/features/codespaces). The `npm install` and `dotnet build` commands described in this README can be skipped if the devcontainer is used.

### GitHub

1. [Fork](https://github.com/diagrid-labs/catalyst-pizza-demo/fork) this repo and clone it locally or use GitHub Codespaces.

#### Ably

Ably is used as the serverless realtime messaging component. There is a default Ably app when you sign up for an account that can be used for this demo. Alternatively, you can create a new Ably app.

1. Log into the [Ably portal](https://ably.com/accounts/).
1. Using the Ably portal: copy the [Root API key](https://ably.com/docs/ids-and-keys#api-key) from the Ably app. This will be copied later as an environment variable for both Vercel and Diagrid.

#### Vercel

The Vue-based [front-end](/front-end/src) and two [JavaScript functions](/front-end/api) are hosted on Vercel. The Vercel CLI is used to configure and run these resources locally.

1. Open a terminal in the root of the repository login with the Vercel CLI:

   ```bash
   vercel login
   ```

1. Go to the `front-end` folder and run:

   ```bash
   npm install
   ```

1. Go back to the root of the repository and setup the Vercel project by running:

   ```bash
   vercel
   ```

    Follow the CLI prompts, and select the following options:
    - Setup and deploy: `Y`
    - Scope: `<your account name>`
    - Link to existing project: `N`
    - What's your project's name? `catalyst-pizza-project`
    - In which directory is your code located? `./front-end`
    - Want to modify these settings? [y/N] `n`
    - Wait for the deployment to complete.

1. An environment variable is used in the *getAblyToken* function to generate a token for the website to communicate with the Ably realtime service. Add the *Ably API token* variable by running `vercel env add`:
    - Variable name: `ABLY_API_KEY`
    - Variable value: *Use the Ably API key obtained from the Ably portal earlier*
    - Select `Development` as the environment.
1. Another environment variable is used in the *placeOrder* function to send a request to the *PizzaOrderService* that will start the workflow. Add the *WORKFLOW_URL* variable by running `vercel env add`:
    - Variable name: `WORKFLOW_URL`
    - Variable value: `http://localhost:5064/workflow/orderReceived`
    - Select `Development` as the environment.
1. Run `vercel pull` to pull the configuration from Vercel to your local environment.
1. Run `vercel build` to build the website and the serverless functions.

#### Diagrid Catalyst

Diagrid Catalyst provides serverless Dapr APIs that enables developers to quickly build distributed applications with workflow, pub/sub messaging, service invocation, and state management capabilities. Diagrid also provides managed infrastructure for storing data in a key/value store, pub/sub messaging, and workflow, which are all used in this solution.

 The [Diagrid CLI](https://docs.diagrid.io/catalyst/references/cli-reference/intro) is used to configure the resources and run the .NET services locally.

1. Open another terminal in the root of the repository and use the Diagrid CLI to login to Diagrid:

   ```bash
   diagrid login
   ```

1. Create a new Catalyst project named `catalyst-pizza-project` and use the Diagrid managed pub/sub broker & KV store, and enable the managed workflow API:

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

1. Before continuing, check the App IDs to make sure they have been created:

	```bash
	diagrid appid list
	```

1. Create a pub/sub subscription for the *KitchenService* to receive messages from the *PizzaOrderService*:

	```bash
	diagrid subscription create pizzaorderssub --component pubsub --topic pizza-orders --route /prepare --scopes kitchenservice
	```

1. Create a pub/sub subscription for the *PizzaOrderService* to receive messages from the *KitchenService*:

	```bash
	diagrid subscription create preparedorderssub --component pubsub --topic prepared-orders --route /workflow/orderPrepared --scopes pizzaorderservice
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

**Update the key/value store to allow state sharing**

The two .NET services both use the same data in the key/value store to manage inventory and orders. Since this is not the default usage of state stores and services when using Dapr, an attribute needs to be set to enable this. The default behavior is that a key is prefixed with the app ID of the service that is using it. In this case, however, the `keyPrefix` is set to `name` to make sure both services use the same keys.

1. Run the following command to update the managed key/value store:

    ```bash
    diagrid component apply -f ./infra/kv.yml
    ```

    This will upload the `kv.yml` file to Diagrid and update the configuration of the *kvstore* component so `keyPrefix` is set to `name`.

### Inspect the DaprClient configuration

The two .NET services use the Dapr .NET client SDK for workflow, and pub/sub messaging. The `DaprClient` is configured to use the endpoints provided by Diagrid Catalyst in the *[Program.cs](back-end\PizzaOrderService\Program.cs)* file:

```csharp
var apiToken = Environment.GetEnvironmentVariable("DAPR_API_TOKEN");
var grpcEndpoint = Environment.GetEnvironmentVariable("DAPR_GRPC_ENDPOINT");
var httpEndpoint = Environment.GetEnvironmentVariable("DAPR_HTTP_ENDPOINT");

builder.Services.AddDaprClient(options => {
    options.UseDaprApiToken(apiToken);
    options.UseGrpcEndpoint(grpcEndpoint);
    options.UseHttpEndpoint(httpEndpoint);
});
```

To use service invocation with the HTTP API, the `HttpClient` is configured with the `DAPR_APP_ID` and `DAPR_API_TOKEN` environment variables:

```csharp
var appId = Environment.GetEnvironmentVariable("DAPR_APP_ID");
var apiToken = Environment.GetEnvironmentVariable("DAPR_API_TOKEN");

builder.Services.AddHttpClient(
    "daprEndpoint", 
    client => {
        client.BaseAddress = new Uri(httpEndpoint);
        client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));
        client.DefaultRequestHeaders.Add("dapr-app-id", appId);
        client.DefaultRequestHeaders.Add("dapr-api-token", apiToken);
    });
```

### Running the solution

1. Open a terminal in the root of the repository.
1. To restore and build the .NET projects run:

   ```bash
   dotnet build ./back-end/PizzaOrderService
   dotnet build ./back-end/KitchenService
   ```

1. Run `diagrid dev start` to start the `PizzaOrderService` and the `KitchenService`.
1. Using another terminal in the root of the repository run `vercel dev` to start the website and the serverless functions locally.
1. Navigate to the URL provided by the Vercel CLI to view the website.
1. Select some pizzas, place an order, and watch the progress of the workflow in realtime.

### Use the Catalyst API explorer

You can use the API explorer in the Catalyst web UI to interact with the managed Dapr APIs. If you want to retrieve the order item from the key./value store that has just been processed follow these steps:

1. Open the browser devtools console of the browser that is running the demo (a pizza order must be started or completed).
1. The order is logged to the console as a JSON object. Copy the `OrderId` property value.

    ![Order](/images/console-order.png)

1. Navigate to the [Catalyst web UI](https://catalyst.diagrid.io). Ensure you're in the `catalyst-pizza-project` project.

    ![API explorer](/images/catalyst-api-explorer.png)

4. Select *App IDs* in and click on the [*API explorer*]([catalyst-pizza-project](https://catalyst.diagrid.io/app-ids/api-explorer/state-api/)) tab.
   - Select the `State API`.
   - Select the `pizzaorderservice` as the app ID.
   - Select `GET` as the API operation.
   - Select `kvstore` as the state component.
   - Enter `Order-<ORDER-ID>` as the key, where `<ORDER-ID>` is substituted with the value copied from the devtools console.
5. Click *Send*. The response should contain the state of the order item with `"status": "CompletedPreparation"`.

![tmnt](/images/tmnt.gif)

## What's Next?

Congratulations! You’ve now used the Diagrid Catalyst serverless Dapr APIs for workflow, pub/sub messaging, service invocation, and state management. The ability to use these APIs from anywhere, without the overhead of managing Kubernetes clusters, brings great flexibility to developers on any platform to build distributed applications. The Dapr applications used in this demo can be hosted on any cloud. The demo uses the Diagrid managed infrastructure for key/value storage and pub/sub messaging, but these can be swapped out for other cloud-based resources, similarly to switching open-source Dapr components. You can extend this demo with an alternative pub/sub broker or state store by configuring other [infrastructure components](https://docs.diagrid.io/catalyst/concepts/components).

Any questions or comments about this demo? Join the [Diagrid Community on Discourse](https://community.diagrid.io/invites/fAUrdyBbie) and post a message in the *Catalyst* category. Have you made something with Catalyst? Post a message in the *Built with Catalyst* category, we love to see your creations!
