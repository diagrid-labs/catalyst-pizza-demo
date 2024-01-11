# Pizza Workflow Demo

```mermaid
graph TD

subgraph State & Messaging
	KV[(KV store)]
	PS[PubSub]
end

subgraph Web services
	KS[Kitchen service]
	WF[Pizza Workflow Service]
end

subgraph Vercel
	W[website]
	PF[placeOrder function]
	RF[getRealtimeToken function]
end

RTS[Realtime service]

W -->|place order| PF
W --> |get token| RF
RF ---->|get token| RTS
PF --> |start workflow|WF
WF-->PS
WF-->KV
PS-->KS
WF---->|notification|RTS
KS-->|ready|PS
PS-->|raise event|WF
RTS ---->|notification| W
```