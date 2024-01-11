export default async function handler(request, response) {
    let data = request.body.json();
    console.log(data);
    let url = `${process.env.WORKFLOW_URL}?instanceId=${data.orderId}`;
    const workflowResponse = await fetch(url, {
        method: "POST",
        mode: "no-cors", // no-cors, *cors, same-origin
        cache: "no-cache",
        headers: {
          "Content-Type": "application/json",
        },
        body: JSON.stringify(data),
      });

    console.log(workflowResponse.json());
    response.status(200);
}