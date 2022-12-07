import requests

data = [

    {

  "Date/Time": "2022-10-30T16:50:28",

  "ID": 4,

  "bbox": [

    50,

    50,

    98.61604309082031,

    111.81181335449219,

    -80.53227233886719

  ],

  "score": 0.9172785878181458

},

{

  "Date/Time": "2022-10-27T16:50:28",

  "ID": 4,

  "bbox": [ 50, 500, 98.61604309082031, 111.81181335449219, -80.53227233886719 ],

  "score": 0.9172785878181458

}

]


# URL for blub storage on cloud
url = "https://raspeberryimagedata.azurewebsites.net/api/data/push"

# Post data
r = requests.post(url, json = data)

