import Ably from "ably/promises.js";
 
export default async function handler(request, response) {
    let params = new URLSearchParams(request.query);
    let clientId = params.get('clientId');
    let client = new Ably.Realtime(process.env.ABLY_API_KEY);
    let tokenRequestData = await client.auth.createTokenRequest({ clientId: clientId });

    response.status(200).json(tokenRequestData);
}