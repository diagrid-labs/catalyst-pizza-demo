export default async function handler(request, response) {
    let order = request.body;
    console.log(order);
    let url = `${process.env.WORKFLOW_URL}`;
    console.log(url);
    const workflowResponse = await fetch(url, {
        method: "POST",
        mode: "no-cors",
        cache: "no-cache",
        headers: {
          "Content-Type": "application/json",
        },
        body: JSON.stringify(order),
      });

    let instanceId = await workflowResponse.json();
    console.log(`instanceId: ${instanceId}`);
    response.status(200).json({ orderId: order.OrderId });
}