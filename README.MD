# Warehouse Scanners API

## What is "Warehouse Scanners API?"
Warehouse Scanners API goal is to connect warehouse remote scanners with data from server. By API requests employe scanner get data about products, order, products locations (storage), and all what is needed to complete order. API allowes to log in on employee account and authorize requests by account token (claims). <br/> 

It's not entire ERP system. We have no possibility to change more constant data, like storages, products types etc.   
Entites should have more complex data inside, like shipment address, invoice data etc. In this project is only, what is needed to work with scanners. 

<br/>

# Example process flow

## Inbound
1. Get empty location for specific pallet

    > **STORAGE GET** /api/storage/empty/palletId=Euro80100

2. Returns empty storage location, valid for pallet size and weight

    > A1-01-B01

3. Take pallet to specific storage
    > **STORAGE POST** /api/storage/pallet?palletId=Euro80100&storageId=A1-01-B01

4. Pallet content is moved to storage content, pallet is removed from database

## Picking
1. Get orders list
    > **ORDER GET** api/order
    ```
    [
        {
            "orderId": "220813054839",
            "created": "2022-08-13T17:48:39.4965552",
            "productsQty": 53,
            "productsWeight": 790.2,
            "productsVolume": 3649.96
        },
        {
            "orderId": "220813085239",
            "created": "2022-08-13T20:52:39.4965589",
            "productsQty": 84,
            "productsWeight": 2062.4,
            "productsVolume": 10039.73
        },
        {
            "orderId": "220813085939",
            "created": "2022-08-13T20:59:39.4965577",
            "productsQty": 69,
            "productsWeight": 3620.9,
            "productsVolume": 17200.35
        }
        (...)
    ]
    ```

2. Choose one order from list
    > **ORDER GET** api/order/220814054339

3. You receive order data
    ```
    {
        "orderId": "220814054339",
        "orderPositions": [
            {
                "productId": "18ActiveSub",
                "pickedQty": 0,
                "toPick": 40,
                "singleWeight": 45
            },
            {
                "productId": "10ActiveSpeaker",
                "pickedQty": 0,
                "toPick": 29,
                "singleWeight": 15
            },
            {
                "productId": "15ActiveSpeaker",
                "pickedQty": 0,
                "toPick": 15,
                "singleWeight": 14.6
            }
        ],
        "pickedPallets": []
    }
    ```

4. Add pallet to order
    > **ORDER POST** /api/order/pallet
    ```
    {
        "id": "220814054339-01",
        "orderId": "220814054339",
        "width": 1000,
        "depth": 1200
    }
    ```

5. Choose product to pick, and find storage location
    > **STORAGE GET** api/storage/product?productId=18ActiveSub&Qty=40

6. Receive product location  
In current case there is not a storage with enough product qty, that's why from current location can be picked only 11/40 pieces. 
    ```
    {
        "storageId": "A1-01-A03",
        "qty": 11
    }   
    ```

7. Pick items 
    > **ORDER PUT** api/order/pick
    ```
    {
        "productId": "18ActiveSub",
        "qty": 11,
        "storageId": "A1-01-A03",
        "palletId": "220814054339-01"
    }
    ```

8. Repeat until order will be complete.

**PICK** Endpoint update order positions, storages, and pallets statuses. When you finish last pick, order status will be changed to completed.
