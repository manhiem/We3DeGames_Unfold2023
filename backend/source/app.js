const express = require('express');
const { auth } = require('express-openid-connect');
const { generateNonce, generateRandomness ,jwtToAddress} = require('@mysten/zklogin') ;
const SuiClient  =require('@mysten/sui.js/client').SuiClient ;
const Ed25519Keypair=require('@mysten/sui.js/keypairs/ed25519').Ed25519Keypair;
const FULLNODE_URL = 'https://fullnode.devnet.sui.io'; // replace with the RPC URL you want to use
const suiClient =  new SuiClient({ url: FULLNODE_URL });
const { epoch, epochDurationMs, epochStartTimestampMs } =  suiClient.getLatestSuiSystemState();
const app = express();
const jwt_decode =require("jwt-decode");
var jwt='';
const packageObjectId = '0x...';

const config = {
  authRequired: false,
  auth0Logout: true,
  secret: 'a long, randomly-generated string stored in env',
  baseURL: 'https://1137-2401-4900-6313-eca5-1929-9927-3835-45cb.ngrok-free.app',
  clientID: 'zQzrtFfDZqcNBYZbbngDaLo1qQp0vj9u',
  issuerBaseURL: 'https://dev-j8j72ld5.us.auth0.com'
};
const maxEpoch = Number(epoch) + 2; // this means the ephemeral key will be active for 2 epochs from now.
const ephemeralKeyPair = new Ed25519Keypair();
const randomness = generateRandomness();
const nonce = generateNonce(ephemeralKeyPair.getPublicKey(), maxEpoch, randomness);
 app.use(auth(config));
 app.all('*', function(req, res, next){
  if(req.oidc.isAuthenticated()){
    console.log(req.oidc.idToken);
    jwt=req.oidc.idToken;
    const decodedJwt = jwt_decode(jwt);
    var dec={iss: decodedJwt.iss,
       sub: decodedJwt.sub,
       aud : decodedJwt.aud,
       exp : decodedJwt.exp,
       nbf : decodedJwt.nbf,
       iat : decodedJwt.iat,
       jti : decodedJwt.jti,}
    
       const zkLoginUserAddress = jwtToAddress(jwt, "134523452")
       console.log(zkLoginUserAddress)
  }

 
  }

);
app.get('/', (req, res) => {
  res.send("got that ")
    
  });

  app.post('/create game', async (req, res) => {
const tx = new TransactionBlock();
tx.moveCall({
	target: `${packageObjectId}::new_game`,
});
const result = await client.signAndExecuteTransactionBlock({
	signer: keypair,
	transactionBlock: tx,
});
if (response?.objectChanges) {
  const newObjectEvent = response.objectChanges.find(
    (e) => e.type === "created"
  );
  if (!newObjectEvent || !('objectId' in newObjectEvent)) return;

  const { objectId } = newObjectEvent;
  
  const cloneTransactionBlock = new TransactionBlock();
  cloneTransactionBlock.moveCall({
    target: `${ETHOS_EXAMPLE_CONTRACT}::example::clone`,
    arguments: [
      cloneTransactionBlock.object(objectId)
    ]
  });

  await wallet.signAndExecuteTransactionBlock({
    transactionBlock: cloneTransactionBlock
  });
  setNftObjectId(objectId)
console.log({ result });
  }}); 
  
  const port = 3000;
app.listen(port, () => {
  console.log(`Server is running on http://localhost:${port}`);
});
