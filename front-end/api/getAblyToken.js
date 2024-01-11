import Ably from "ably/promises.js";
 
export default async function handler(request, response) {
    let params = new URLSearchParams(request.query);
    let clientId = params.get('clientId');
    let client = new Ably.Realtime(process.env.ABLY_API_KEY);
    let tokenRequestData = await client.auth.createTokenRequest({ clientId: clientId });

    //console.log(`Request: ${JSON.stringify(tokenRequestData)}`);
    response.status(200).json(tokenRequestData);
}