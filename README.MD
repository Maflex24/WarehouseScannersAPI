# Warehouse Scanners API

## What is "Warehouse Scanners API?"
Warehouse Scanners API goal is to connect warehouse remote scanners with data from server. By API requests employe scanner get data about products, order, products locations (storage), and all what is needed to complete order. API allowes to log in on employee account and authorize requests by account token (claims). <br/> 

<br/>

It's not entire ERP system. We have no possibility to change more constant data, like storages, products types etc. 
  
***  
  
## General actions:
- *Account*
    - log-in to system
    - change password possibility
<br/> <br/>

- *Inbound* - after receipt of goods, pallet need to be deliver on storage place. API need to give information about empty storage place, compatible with pallet requirement (weight, height).  
After deliver pallet on indicated storage place, API need to save information what product, and what amount is on specific storage.
    - return empty storage location, based on pallet data
    - save product on storage
<br/> <br/>

- *Picking*
    - return list of avalaible orders to pick
    - return data of specific order
    - sort orders position descending by weight and/or size
    - return location of next product on order positions list
    - get picker input, if quantity, product, and location is correct, save position as picked
    - next location
    - save order as completed

