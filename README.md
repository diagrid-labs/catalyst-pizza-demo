# Pizza Workflow Demo

![tmnt](/images/tmnt.gif)

This repository contains a solution that demonstrates how to use combine Dapr APIs for workflow,  pub/sub, and state management to build a distributed pizza ordering system.



The solution includes:

- [Vercel](https://vercel.com); to host the website (based on Vue) and two serverless functions (JavaScript).
- Two [Dapr](http://dapr.io) services written in .NET, _Pizza Order Service_ and _Kitchen Service_.
- [Ably](https://ably.com/); to provide realtime messaging between the website and backend Pizza Order Service.

```mermaid
graph TD

subgraph State & Messaging
	KV[(KV store)]
	PS[PubSub]
end

subgraph Web services
	KS[Kitchen service]
	WF[Pizza Order Service]
end

subgraph Vercel
	W[website]
	PF[placeOrder function]
	RF[getRealtimeToken function]
end

RTS[Realtime service]

W -->|place order| PF
W --> |get auth token| RF
RF ---->|get token| RTS
PF --> |start workflow|WF
WF-->|send order to kitchen|PS
WF-->|check inventory|KV
KS-->|update inventory|KV
PS-->|receive order|KS
WF---->|notification|RTS
KS-->|order prepared|PS
PS-->|raise order prepared event|WF
RTS ---->|website notification| W
```

The repo contains two variations:

1. The `local-dapr` branch runs the .NET services locally and uses Dapr in [self-hosted mode](https://docs.dapr.io/operations/hosting/self-hosted/self-hosted-overview/) using the Dapr CLI with multi-app-run.
2. Work in progress: The 'main' branch runs the .NET services on Google Cloud Run and uses a managed version of the Dapr API provided by [Diagrid Catalyst](https://www.diagrid.io/catalyst).

