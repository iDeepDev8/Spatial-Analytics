# Library imports
import json
import time
from Get_Latest_Data import GetLatestData

# Pre-defined co-ordinated to evenly sub-section images into four equal regions
WorkBench1 = [0, 0, 320, 240]
WorkBench2 = [320, 0, 640, 240]
WorkBench3 = [0, 240, 320, 480]
WorkBench4 = [320, 240, 640, 480]

# Array containing names to the four regions
ROIS = [WorkBench1, WorkBench2, WorkBench3, WorkBench4]
ROI_Names = ["Region 1", "Region 2", "Region 3", "Region 4"]

# Function to detect overlap of bounding boxes and the centriod of the points
def DetectOverlap_2(Boundb, Point):

   IsOccupied = False

   # If top-left point corner is inside the bounding box
   if Boundb[0] < int(Point[0]) and Boundb[1] < int(Point[1]):

      # If bottom-right point corner is inside the bounding box
      if int(Point[0]) + 1 < Boundb[0] + Boundb[3] \
              and int(Point[1]) + 1 < Boundb[1] + Boundb[2]:
         #print('The point is in BB')
         IsOccupied = True

         # (IsOccupied)
      else:
         pass
         #  pass#print('Some part of the box is outside the bounding box.')
         IsOccupied = True

   else:
      pass#print('The point is not in the BB')

   return IsOccupied

# Function to detect overlap of bounding boxes and the centriod of the points
def DetectOverlap(Boundb, Point):
   IsOccupied = Boundb[0] < Point[0] < Boundb[0] + Boundb[2] and Boundb[1] < Point[1] < Boundb[1] + Boundb[3]
   return IsOccupied


def SubSectioning(Data):

   # Variable Definition
   Points = Data
   iteration = 0
   Overlap = []

   # Nested loop runs through all of the ROIS and then runs through all points

   # Iterating through the Rois
   for Roi in ROIS:

      Bool_Detected = False

      # Iterate through the detected points
      for Point in Points:

         # Run function to Detect Overlaps passing in the Roi and Points
         Detect = DetectOverlap(Roi, Point)

         if Detect == True:
               Bool_Detected = True


      if Bool_Detected == True:
         #print(str(ROI_Names[iteration]) + " region is occupied")

         Overlap.append(True)

      else:
         #print(str(ROI_Names[iteration]) + " region is not occupied")

         Overlap.append(False)


      iteration = iteration + 1

   return Overlap