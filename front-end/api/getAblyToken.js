import Ably from "ably/promises.js";
 
export default async function handler(request, response) {
    let client = new Ably.Realtime(process.env.ABLY_API_KEY);
    let tokenRequestData = await client.auth.createTokenRequest({ clientId: 'ably-token-function' });

    //console.log(`Request: ${JSON.stringify(tokenRequestData)}`);
    response.status(200).json({
        data: tokenRequestData
    });
}