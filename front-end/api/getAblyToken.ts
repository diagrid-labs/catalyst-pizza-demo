import type { VercelRequest, VercelResponse } from '@vercel/node';
import * as Ably from "ably/promises";
 
export default async function handler(
  request: VercelRequest,
  response: VercelResponse,
) {

    let clientOptions: Ably.Types.ClientOptions = { key: process.env.ABLY_API_KEY };
    let client = new Ably.Rest(clientOptions);
    let tokenRequestData = await client.auth.createTokenRequest({ clientId: 'ably-token-function' });

    console.log(`Request: ${JSON.stringify(tokenRequestData)}`)
    let token = Response.json(tokenRequestData);
  
    response.status(200).json({
      body: token,
    });
}