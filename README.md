# simple-debugger

Allow debugging LCU API calls via Fiddler

## UPDATE
4th May 2023: To build do not use AnyCPU, make sure it is set to x64 release !

## 
Remember to:
  * Wait for the client to open
  * Run Fiddler as Administrator
  * Tool > Options >  tick "Decrypt https traffic" 
  
  ![immagine](https://user-images.githubusercontent.com/8062792/159447174-361b8a65-6fb6-4413-b4c5-73beece4b9fd.png)
  
  Now you will be able to see api calls from 127.0.0.1:PORT
  
  I recommend adding this ``127.0.0.1;`` in the Hosts (Filters tab) and "Show only the following Hosts"
  
  or you will get spammed by everything. If you want to see the store's calls, disable the filter.
  
  ![immagine](https://user-images.githubusercontent.com/8062792/159447749-f6bc42eb-01d5-470a-8621-8baf7e3acde0.png)

