# Mass Token Transfer

Ride smart contract for mass token transfer

## dApp-based transfer vs Mass Transfer Transaction

| Criterion       |  dApp-based   |           Mass Transfer Transaction            |
|-----------------|:-------------:|:----------------------------------------------:|
| Assets quantity | from 1 to 10  |                       1                        |
| Minimum fee     |  0.005 WAVES  | 0.001 + 0.0005 × N WAVES (3 decimals rounding) |
| Sponsored fee   |    Support    |                   No support                   |
| Recipients      | from 1 to 100 |                 from 1 to 100                  |
| Attachment      |    Support    |                    Support                     |

## Interface

`@Callable(i) func massTransfer(recipients: List[ByteVector], amounts: List[Int], paymentIdx: List[Int], attachment: String)`

- A recipient is represented as the j-th element of each of the arrays `recipients`, `amounts` and `paymentIdx`
- `recipients: List[ByteVector]` - 26 byte array for each recipient
- `amounts: List[Int]` - amount of assets to be transferred for each j-th recipient
- `paymentIdx: List[Int]` - payment index from `i.payments` array which corresponds to j-th amount
- `attachment: String` - any string attachment

### Transaction example

In the following example 5000000000 WAVELET and 8000000 TOKEN distributed between two recipients:

- base64:AVKNb1d7eGRvvO0FTuZ/UMM/XLmuIt1e3Qw= receives 1000000000 WAVELET and 8000000 TOKEN
- base64:AVJUC7Ul63EL7nfgO7tOyUWZ6M5zbXNu7wk= receives 4000000000 WAVELET

```json
{
	...
    "payment": [{
            "amount": 5000000000,
            "assetId": null
        }, {
            "amount": 8000000,
            "assetId": "83YjMHjjJ53hy15MsXAFMQuZSaKvdXuixjiTWFMmMqfD"
        }
    ],
    "call": {
        "function": "massTransfer",
        "args": [{
                "type": "list",
                "value": [{
                        "type": "binary",
                        "value": "base64:AVKNb1d7eGRvvO0FTuZ/UMM/XLmuIt1e3Qw="
                    }, {
                        "type": "binary",
                        "value": "base64:AVKNb1d7eGRvvO0FTuZ/UMM/XLmuIt1e3Qw="
                    }, {
                        "type": "binary",
                        "value": "base64:AVJUC7Ul63EL7nfgO7tOyUWZ6M5zbXNu7wk="
                    }
                ]
            }, {
                "type": "list",
                "value": [{
                        "type": "integer",
                        "value": 1000000000
                    }, {
                        "type": "integer",
                        "value": 8000000
                    }, {
                        "type": "integer",
                        "value": 4000000000
                    }
                ]
            }, {
                "type": "list",
                "value": [{
                        "type": "integer",
                        "value": 0
                    }, {
                        "type": "integer",
                        "value": 1
                    }, {
                        "type": "integer",
                        "value": 0
                    }
                ]
            }, {
                "type": "string",
                "value": "Test attachment"
            }
        ]
    },
	...
}
```

## Deploy

Mainnet: https://wavesexplorer.com/addresses/3PACv5M5MXkWMd9881rmUL7pV7tokQGckZU

Testnet: https://wavesexplorer.com/addresses/3N73f75kdpYSdhqarJ7xkYGdGj8vZwwk9ta?network=testnet

## Run tests

### Install docker

Link: https://docs.docker.com/engine/install/

### Install dotnet sdk

Link: https://learn.microsoft.com/en-us/dotnet/core/install/

### Run waves private node in docker

Link: https://github.com/wavesplatform/Waves/tree/HEAD/docker

Example:

```bash
docker run --rm -d --name node -p 6869:6869 wavesplatform/waves-private-node
```

### Execute tests

Execute the following command from the project root:

```bash
dotnet test ./tests --nologo -l "console;verbosity=normal"
```