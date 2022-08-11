# Warehouse Scanners API

## What is "Warehouse Scanners API?"
Warehouse Scanners API goal is to connect warehouse remote scanners with data from server. By API requests employe scanner get data about products, order, products locations (storage), and all what is needed to complete order. API allowes to log in on employee account and authorize requests by account token (claims). <br/> 

<br/>

It's not entire ERP system. We have no possibility to change more constant data, like storages, products types etc.   
Entites should have more complex data inside, like shipment address, invoice data etc. In this project is only, what is needed to work with scanners. 
  
***  


## "Storage" endpoints:

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

