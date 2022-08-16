# Warehouse Scanners API

## What is "Warehouse Scanners API?"
Warehouse Scanners API goal is to connect warehouse remote scanners with data from server. By API requests employe scanner get data about products, order, products locations (storage), and all what is needed to complete order. API allowes to log in on employee account and authorize requests by account token (claims). <br/> 



It's not entire ERP system. We have no possibility to change more constant data, like storages, products types etc.   
Entites should have more complex data inside, like shipment address, invoice data etc. In this project is only, what is needed to work with scanners. 

<br/>

# Endpoints

## Account endpoints

> **POST** /api/account/login

When user log-in sucesfully it returns jwt token, used to authorization any other API operation.

> **POST** /api/account/password

Allow user to change password 

<br/> 

## Storage endpoints

> **GET** /api/storage/empty

Based on `PalletId` returns valid, empty storage location, where we can fit pallet. Service check:
- are pallet dimensions smaller than storage dimensions
- isn't pallet too heavy for specific storage
- storage is smallest as possible, to not waste huge space for small pallets. 

> **POST** /api/storage/pallet

If `Storage` and `Pallet` data are valid, API assign products from pallet to storage. Then pallet, and pallet content are removed. All pallet content is assign to storage space now. 

> **GET** /api/storage/product

Based on `productId`, and `Qty` API search for product location, started from smallest qty values (to not get products from fresh storage, if it's enough in other place). <br/>
If there is not enough product amount on any location, API return location where is most qty of this product. That's why API returns `Qty` value too, not only `storageId`.

<br/>

## Order endpoints

> **GET** api/order

Returns orders list, ordered by creation date. Have information about total products qty, volume, and weight.

> **GET** api/order/{orderId}

Returns details about specific order, requaires to start picking 

> **PUT** api/order/pick

Allowes to move products from storage to pallet of specific id, and order. Program check conditions and update storage, pallet and order data. 

> **POST** api/order/pallet

Creates new pallet for specyfic order. When picker picking need to assign product to some pallet. 

<br/>

# Example process flow

## Inbound
1. Get empty location for specyfic pallet

    > **STORAGE GET** /api/storage/empty/palletId=Euro80100

2. Returns empty storage location, valid for pallet size and weight

    > A1-01-B01

3. Take pallet to specific storage
    > **STORAGE POST** /api/storage/pallet?palletId=Euro80100&storageId=A1-01-B01

4. Pallet content is moved to storage content, pallet is removed from database

## Picking
1. Get orders list
    > **ORDER GET** api/order

2. Pick order from list
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

4. Choose product to pick, and find storage location
    > **STORAGE GET** api/storage/product?productId=18ActiveSub&Qty=40

5. Receive product location  
In current case there is not a storage with enough product qty, that's why from current location can be picked only 11/40 pieces. 
    ```
    {
        "storageId": "A1-01-A03",
        "qty": 11
    }   
    ```

6. Pick items 
    > **ORDER PUT** api/order/pick
    ```
    {
        "productId": "18ActiveSub",
        "qty": 11,
        "storageId": "A1-01-A03",
        "palletId": "220814054339-01"
    }
    ```

7. Repeat until order will be complete.

**PICK** Endpoints update order positions, storage, and pallet statuses. When you finish last pick, order status will be changes to completed.
