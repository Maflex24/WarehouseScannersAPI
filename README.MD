
# Warehouse Scanners API

## What is "Warehouse Scanners API?"
Warehouse Scanners API goal is to connect warehouse remote scanners with data from server. By API requests employe scanner get data about products, orders, products locations (storage), and all what is needed to complete order. API allowes to log in on employee account and authorize requests by account token. 

It's not entire ERP system. We have no possibility to change more constant data, like storages, products types etc.   
Entites should have more complex data inside, like shipment address, invoice data etc. In this project is only, what is needed to work with scanners. 


# Features

- Account authorization, you need to log in, to do other requests
- Get list of existed orders with information about total products ordered, total products weight, and volume. You can get orders from specific day only, if you need
- Get order details, the same what above, and ordered products list, with qty, how many left to pick, and info about pallets assigned to this order
- Create new pallet, assigned to the specific order
- Find storage location for product. If there is no location with requested quantity, you will receive storage location with max quantity avalaible
- Assign (pick) product to order. You will assign product from specific storage and quantity to pallet, which is assigned to order
- Find empty storage place for existed pallet, based on pallet and storage dimensions and weight
- Assign pallet to empty storage place


# How to run

Clone project

```git
git clone https://github.com/Maflex24/WarehouseScannersAPI.git
```

At `appsettings.json` file change database connection strings for valid to your environment

```json
"ConnectionStrings": {
  "WarehouseDbConnectionString": "Server=(LocalDb)\\LocalDbServer;Database=WarehouseScannersDb;Trusted_Connection=True;"
},
```

In nuget package manager console type:

```bash
update-database
```

Then Entity Framework will create database structure for you 

Run project

```bash
dotnet run
```

Better if you run project at developer environment, by using `Visual Studio` for example. Then you get swagger documention on browser, and will be easier to testing.  

Program has build-in data seeder, when your database is configured corretly, seeder should fill sample data.


# Example process flow

First you need to log-in, to authorize all request by received jwt token

```http
POST /api/account/login
```

With credentials
```json
{
  "login": "double",
  "password": "123456"
}
```

By default there are three accounts
- `picker` -  has access to picking actions only
- `inbound` - has access to inbound actions only
- `double` - has access to all actions, easiest for testing.  

All builed accounts has password `123456`  


## Inbound
Get empty location for specific pallet

```http
GET /api/storage/empty/palletId=Euro80100
```

You will get empty storage location id, valid for pallet size and weight

```text
 A1-01-B01
```

Take pallet to specific storage, and assign pallet to this storage

```http
POST /api/storage/pallet?palletId=Euro80100&storageId=A1-01-B01
```

Pallet content is moved to storage content. Pallet is removed from database

## Picking
Get orders list

```http
GET /api/order?ResultPerQuery=3&Page=1&Created=2022-08-18
```

You receive orders data:
```json
{
  "totalOrders": 14,
  "totalPages": 5,
  "ordersFrom": 1,
  "ordersTo": 3,
  "orders": [
    {
      "orderId": "220818-121225-43",
      "created": "2022-08-18T00:12:25.6119263",
      "productsQty": 90,
      "productsWeight": 3997.1,
      "productsVolume": 20191.719
    },
    {
      "orderId": "220818-012314-35",
      "created": "2022-08-18T01:23:14.6119227",
      "productsQty": 81,
      "productsWeight": 1170.2,
      "productsVolume": 6688.16
    },
    {
      "orderId": "220818-030317-03",
      "created": "2022-08-18T03:03:17.611901",
      "productsQty": 15,
      "productsWeight": 225,
      "productsVolume": 831
    }
  ]
}
```

Choose one order from list

```http
GET /api/order/220818-030317-03
```

You receive order data
```json
{
  "orderId": "220818-030317-03",
  "productsWeightLeft": 225,
  "productsVolumeLeft": 831,
  "orderPositions": [
    {
      "productId": "10ActiveSpeaker",
      "pickedQty": 0,
      "toPick": 15,
      "singleWeight": 15
    }
  ],
  "pickedPallets": []
}
```

Add pallet to order

```http
POST /api/order/pallet
```

With body:

```json
{
  "id": "220818-030317-03",
  "orderId": "220818-030317-03",
  "width": 800,
  "depth": 1200
}
```

Choose product to pick, and find storage location

```http
GET /api/storage/product?productId=10ActiveSpeaker&Qty=15
```

You will get storage id (location), and avalaible quantity as response

```json
{
  "storageId": "A1-09-C01",
  "qty": 15
}
```

If there will be not location storage with your requested qty, you will get location with max avalaible storage qty. Then you need to pick from this location and do request again, to find new storage.  


Pick items

```http
POST /api/order/pick
```

Example body:

```json
{
  "productId": "10ActiveSpeaker",
  "qty": 15,
  "storageId": "A1-09-C01",
  "palletId": "220818-030317-03"
}
```

When you do again request

```http
GET /api/order/220818-030317-03
```

Order data will be updated

```json
{
  "orderId": "220818-030317-03",
  "productsWeightLeft": 0,
  "productsVolumeLeft": 0,
  "orderPositions": [],
  "pickedPallets": [
    {
      "palletId": "220818-030317-03",
      "weight": 225
    }
  ]
}
```

At current case order is finished. If it's not, you need to repeat process, untils it's done.    
# API Reference

## Account 

### Log-in

```http
POST /api/account/login
```

| Source | Parameter | Type |
| :----- | :-------  | :------------------------- | 
| `body` | `login` | `string` | 
| `body` | `password` | `string` | 

### Change password

```http
POST /api/account/password
```

| Source | Parameter | Type |
| :----- | :-------  | :------------------------- | 
| `body` | `oldPassword` | `string` | 
| `body` | `newPassword` | `string` | 
| `body` | `confirmNewPassword` | `string` | 


## Storage

### Get empty storage

```http
GET /api/storage/empty
```

| Source | Parameter | Type |
| :----- | :-------  | :--------- | 
| `query` | `palletId` | `string` | 

### Assgin pallet to storage

```http
POST /api/storage/pallet
```

| Source | Parameter | Type |
| :----- | :-------  | :--- | 
| `query` | `palletId` | `string` |
| `query` | `storageId` | `string` |

### Get product location

```http
GET /api/storage/product
```

| Source | Parameter | Type |
| :----- | :-------  | :--------- | 
| `query` | `productId` | `string` | 
| `query` | `Qty` | `integer` |

## Oder

### Get non completed orders

```http
GET /api/order
```

| Source | Parameter | Type |
| :----- | :-------  | :--------- | 
| `query` | `ResultsPerQuery` | `integer` | 
| `query` | `Page` | `integer` |
| `query` | `Created` (optional) | `date` |

### Get order details

```http
GET /api/order/{orderId}
```

| Source | Parameter | Type |
| :----- | :-------  | :------- | 
| `path` | `orderId` | `string` | 


## Tech Stack

- ASP.Net Core 6
- Entity Framework Core 6
- Microsoft SQL Server Database
- Microsoft.AspNetCore.Authentication.JwtBearer 6.0.6
- Swashbuckle.AspNetCore 6.2.3 
- NLog.Web. 5.1

