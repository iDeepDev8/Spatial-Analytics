# Welcome to the Datacom Spatial Analytics wiki!
Here you can find all the necessary information required to get this project set up and running.

# Table of Contents:

### Overview

### Requirements

### Getting Started

### RAPID

### Streamlit

### Use cases

### References

# Overview

The goal of this project is to be able to detect and analyze crowds within a building, and create inferences/applications alongside the detections to further improve the use case

# Requirements

- Raspberry Pi 4 with 64 bit Rasbian OS installed
- Fisheye lens camera for Raspberry Pi (Plugged in)

# Getting Started

## Updating time/Getting the camera going:

Once the Raspberry Pi has been set up after first installing the OS, it is likely that the device's date/time will not be up to date. it's important that the time is set properly due to the server thinking that the incoming connection when using pip install is from another location causing it to get denied.
If the date and time aren't synced properly you can follow these steps to update them:

sudo apt install htpdate

sudo apt update (If this command doesn't work just carry on, it is probably already updated)

sudo reboot

Type date in the console, it should now be updated and correct

Now you will need to check whether or not the raspberry pi is detecting the camera connected to it (Most likely not initially) here: https://webcamtests.com/
If it isn't then enter these commands in the terminal:
pip3 install --user meson
pip3 install --user --upgrade meson

git clone https://git.libcamera.org/libcamera/libcamera.git
cd libcamera
meson build
ninja -C build install

Then go to this link again to ensure it is working with sources outside of libcamera:
https://webcamtests.com/

If it still isn't connected please refer to the libcamera documentation to install it: 
https://libcamera.org/getting-started.html

## Enabling Headerless Mode/VNC Server:

Open the Menu at the top left of the screen, and go to the preferences section, now go to Raspberry Pi Configuration. 
Go to the display tab press enter, go to the VNC tab press enter, then set the headless resolution to 1920x1080 (In some cases the headless resolution will be an option after display skipping the VNC tab altogether). 

Download VNC Server (ARM64 for Raspberry Pi) from this link:
https://www.realvnc.com/en/connect/download/vnc/
From the same link register for an account (make sure you are willing to share the details with someone else, if other people will need to access the Raspberry Pi remotely.), open the file to begin the install. Get through the confirmation prompts, then log into the VNC account you just made when asked to.

At the top left of the screen on the Raspberry Pi, click the button that says VNC (next to the wifi, bluetooth, and volume). 
In the new interface where it says "Sign in to enable cloud connectivity", click the sign in button, then log in with the account that was made earlier. 
Select all the boxes that are on the screen (Allow cloud connections, and Allow direct connections). 

In the next menu click the option that says "Choose VNC password", and set it to a password others can use if they want to access the Pi remotely (pi123456 in our case). Click next, leave the next screen as default and click next again. 

Go to the terminal and run "sudo apt update" then "sudo apt reboot", then turn the Raspberry Pi off and unplug the HDMI connection.

Install VNC Viewer on whichever device you plan on accessing the Raspberry Pi from, now open it up and log into the same account you signed into on the Raspberry Pi, Go to the team section then click the search bar.

Now turn the Raspberry Pi back on and on the device with VNC Viewer is installed, type the name of the Raspberry Pi (In our case just "pi", probably not the best name), then press enter and you should get remote access to the Raspberry Pi 


## Cloning Repo/Installing Requirements:

Clone the repo from this link on the Raspberry Pi:
https://github.com/datacom-spatial-analytics/ComputerVision

Once done run 
pip3 install -r Requirements.txt
This should successfully install all the libraries. 
If the Pytorch install fails please make sure the OS on the Raspberry Pi is the 64-bit version. This is because Pytorch isn't avaliable on a 32-bit system. 
Another reason could be that the Raspberry Pi has defaulted with a newer version of Python which isn't yet compatible with Pytorch, if this is the case you will need to change the python version to something compatible

Find more on recommended Python versions here:
https://pytorch.org/get-started/locally/
Find out how to change Python version here:
https://installvirtual.com/how-to-install-python-3-8-on-raspberry-pi-raspbian/

It should now all be complete, to run the project and ensure everything is going as it should be, go to the terminal with the directory leading to the ComputerVision/Project folder and enter:
python example.py

As long as it starts running without any issues/errors you should be good to connect via vnc viewer and run the program remotely.

# Use cases 

## Occupancy Detection 

### Step 1: Capture Images and get coordinate data
```
File name: Example.py 

1) Read Fisheye lense camera image
2) Apply RAPID over image to detect people
3) Get the detected personals coordinates
4) Save the x-y coordinates into an array
5) Save array into a Json

```

### Step 2: Send this data to the cloud storage 
```
File name: Push_Data_To_Cloud.py

1) Get URL to post data to cloud storage 
2) Run POST request to the URL - sending the data produced by Example.py 

```

### Step 3: Retrieve latest data from cloud storage 
```
File name: Get_Latest_Data.py

1) Get URL to where the data is exposed from the cloud storage
2) Run GET request to the URL - retrieving latest JSON file from cloud
3) Apply Data post-processing to obtain all the X & Y coordinate data and not anything else 

```
### Step 4: Subsection Image into ROIs and detect overlap of the ROI coordinates with the detected personal coordinate data
```
File name: Subsectioning.py

1) Identify ROIs in the Image (Desks, Workspaces, Benches, Kitchens)
2) Get the pixel coordinates for each of the ROIs - this is only needed to be done once (For this you need the orginal image)
3) Read CSV file containing the coordinates
4) Detect the overlap between the coordinates of the detected personal and the pixel coordinates of the ROIs
5) Check if the overlap is present if so append "True" to an array if there is no overlap append "False" to the array
6) Save array into JSON filw

```

### Step 5: Visualize overlap which is the occupancy onto Streamlit 
```
File name: Meeting_Room_Occupancy.py

1) Read CSV which contains an array with the occupancy state of the ROIs
2) Iterate through the array checking the occupancy state
3) If the state is True - Draw a red square on the Datacom floor image in the particular region using custom coordinates and OpenCV
4) Display Image on Streamlit APP

```

# Streamlit

## App Functionalities in each page

### Heatmap Slider
```
File name: Heatmap_Slider.py

1) Display st.slider with start and end point of the dataset time range formatted in "hh:mm:ss"
2) Set variables date(date the dataset was acquired), startTime(first time in the dataset), endTime(last time in the dataset)
3) Send request to Azure storage to access merged json files with data(Date/Time, ID, bbox, score)
4) Post data retrieved from Azure Storage link: 
https://raspeberryimagedata.azurewebsites.net/api/data/merged/all
5) Change txt file retrieved from Azure Storage to json object
6) Iterate through json objects and create an array of bounding boxes with bbox keys
7) Get first elements of each list in the array
8) Normalize the data attributes of bounding box array elements
9) Create a mesh to plot into
10) Display created heatmap using pyplot
11) Write data in json object
```

### Microsoft Power Bi interactive plotting
```
File name: Interactive_Plotting.py

1) Display title of the page 'Interactive Tidal Effect Plot'
2) Create button 'Open Plot' that opens an interactive plots made in Power Bi with Wi-Fi signal data captured by Raspberry Pi placed in the office in order to analyse and visualise occupancy of the building over time
```

### Application of Streamlit
```
File name: Spatial_Analytics.py

1) Display title of the page 'Spatial Analytics'
2) Display image representing spatial analytics project in home page
```

