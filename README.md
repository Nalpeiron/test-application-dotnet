# Zentitle2 On-premises Demo

This application supports .NET 7.0.

## Introduction

This is a demo application for Zentitle2 on-premises. It demonstrates how to integrate Zentitle2 with a .NET application.

## Setup new product

Online activation:

1) Add new product with edition and offering
2) Add features with the following keys: AAC, FLAC, MP3, OGG, OPUS, WAV
3) Add string attributes with following keys: Company, Plan
4) Add usage count adv. feature with key: FilesToConvert
5) Add element pool adv. feature with key: Threads
6) Add entitlement and activate if necessary
7) Turn on features (set them to true), setup number of Threads and FilesToConvert.
8) Fill company and plan attributes
9) Use entitlement 'Activation Code' to activate demo. (Menu license on top right then activate)

Offline activation:

Offline activations offer unlimited conversions with five threads and all extensions. 
No feature setup required. But you can use the same entitlement as online activation, but note that feature setup will be disregarded.

1) Add new product with edition and offering
2) Add string attributes with following keys: Company, Plan
3) Add entitlement and activate if necessary.
4) Set offline lease period for created entitlement.
5) Fill company and plan attributes
6) Use entitlement 'Activation Code' to activate demo. (Menu license on top right then activate)

## Setup App.config

Open the `App.config` file from the main directory.

`SeatId` - can be set to any string value with a maximum length of 50 characters.
`ProductId` - copy the Product id from the Zentitle UI. It can be found on the Entitlement or Product details tab.

This information can be found in the Auth0 UI in the Application settings tab:
`Auth0Domain` - set your Auth0 Domain.
`Auth0ClientId` - set your Auth0 Client id.

These parameters can be located in the Zentitle2 UI. Navigate to the 'Account' menu, select 'API Credentials', and look in the 'Licensing API Details' box:
`TenantId` - copy the TENANT ID value from the Zentitle2 UI.
`ZentitleUrl` - copy the API URL value from the Zentitle2 UI.
`TenantPublicKey` - copy the RSA Modulus (n) value from the Public Key row in the Zentitle2 UI.

```
 <appSettings>
   <add key="SeatId" value="User1-1"/>
   <add key="ProductId" value="prod_tc2a6qe1wEiZRS8uOmZaoQ"/>
   <add key="TenantId" value="t_KRwhRp1yl0_9g1ZjE5YjBw"/>
   <add key="ZentitleUrl" value="https://license.zentitle-test.com"/>
   <add key="Auth0Domain" value="zentitle.eu.auth0.com" />
   <add key="Auth0ClientId" value="tHvAe2rKhlIheEwAUvugUDBHtkutvjw6" />
   <add key="TenantPublicKey" value="o0eQ_zI_f2UDScMhpN1AX7QQQIDtVtP_TvNgHw9H75maThRbIFwGFHkoL-ckPSKUpaduFplshRw1jv_TvJvPMwG78B2WtqGRZ_s-b7cjhKsCLPmmjQsXqoJX62FzelI8WgthxcmHpw0PabBDh2q5hYVkLbLmpAX-bvvAzbIrpNVZtoJLW7l8aNDGY6J3mLRmO6xohcTGa4j_4n-lGC6nuLJGP_fdU9ewPtLanBO0OBw_zemZVH2xlVmaLTXoCwGf7B6rIpqKxed-823BCmx3-bUyfxqrQJ3H_SKP-ublkNIhpEgX73MYCYrt9_9l3Ydr9cXF-_pASFKHR63bEitzfP" />
 </appSettings>
```

