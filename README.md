# Orion On-premises Demo

## Setup new product

1) Add new product with edition and offering
2) Add features with the following keys: AAC, FLAC, MP3, OGG, OPUS, WAV
3) Add string attributes with following keys: Company, Plan
4) Add consumption token with key: FilesToConvert
5) Add element pool with key: Threads
6) Add entitlement and activate if necessary
7) Turn on features (set them to true), fill company and plan attributes. Setup number of Threads and FilesToConvert.
8) Use entitlement 'Activation Code' to activate demo. (Menu license on top right then activate)

##Setup App.config

Open App.config file from main directory

SeatId - can be set to any string value with max length 50 characters
ProductId - copy product id from Orion UI
TenantId - set tenantId
ZentitleUrl - set your license api URL

