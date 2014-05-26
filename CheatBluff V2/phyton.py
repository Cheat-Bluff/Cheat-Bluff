import numpy
import matplotlib.pyplot as plt
import time
import re

rootFolder = ''
skeletonLogger_filename = 'skeletonLog.csv'
faceLogger_filename = 'faceLogger.csv'
anvilAnnotation_filename = 'movieannotation.txt'
accelerometer_filename = 'empatica/ACC.csv'
temperature_filename = 'empatica/TEMP.csv'
heartrate_filename = 'empatica/BVP.csv'
skinconductance_filename = 'empatica/EDA.csv'

ouput_filename='../../ouputresults.txt'

#videostart = '17:28:13' #Ashley Costa - Ashley R1
#videostart = '17:34:31' #Ashley Costa - Ashley R2
#videostart = '17:28:05' #Ashley Costa - Costa R1 ??
#videostart = '???' #Ashley Costa - Costa R2 ??
#videostart = '16:45:00' #Phil Hector - Hector R1
#videostart = '17:04:08' #Phil Hector - Hector R2
#videostart = '16:45:01' #Phil Hector - Phil R1 ??
#videostart = '17:03:57' #Phil Hector - Phil R2 ??
#videostart = '15:48:06' #Diane Luke - Diane R1
#videostart = '16:13:41' #Ryan Luke - Luke R1
videostart = '16:17:53' #Ryan Luke - Luke R2

videostartSecs =  (int(videostart.split(':')[0])*60*60) + (int(videostart.split(':')[1])*60) + float(videostart.split(':')[2])


def read_skeletonLogger(filename):
    f = open(filename,'r')
    lines = f.readlines() #python list of strings
    f.close()

    header = lines[0].split(',') #python list of strings 

    #print header
    
    frames = []
    frames_per_second = 30.0
    
    
    lastTime = -100
    frameNumber = 0
    
    for line in lines[1:]:
        line_split = line.split(',')
        
        if len(line_split[0].split(' ')) > 1 :
            time_split = line_split[0].split(' ')[1].split(':');
            time_seconds =  (int(time_split[0])*60*60) + (int(time_split[1])*60) + float(time_split[2])
                
            
            if time_seconds == lastTime:
                frameNumber = frameNumber + 1
            else:
                lastTime = time_seconds
                frameNumber = 0
                
            time_seconds = time_seconds + (frameNumber / frames_per_second)
            
            if time_seconds >= videostartSecs:
                frame = [];
                frame.append(time_seconds - videostartSecs)
                frame.extend(line.split(',')[1:])
                
                frames.append(frame)

    skeleton_array = numpy.array(frames).astype(float)
    
    return skeleton_array


 
def read_empaticafile(filename):
    f = open(filename,'r')
    lines = f.readlines() #python list of strings
    f.close()

    frames = []
    
    start_time = time.strftime('%H:%M:%S', time.gmtime(float(lines[0].split(',')[0]) )).split(':')
    start_time_in_secs = ((int(start_time[0]) + 2 )*60*60) + (int(start_time[1])*60) + float(start_time[2])
    
    frames_per_second = float(lines[1].split(',')[0])
    
   
    cnt = 0
    
    for line in lines[2:]:
        line_split = line.split(',')
        
        if len(line_split) > 0 :
            time_seconds =  start_time_in_secs + (cnt / frames_per_second)           
            
            if time_seconds >= videostartSecs:
                frame = [];
                frame.append(time_seconds - videostartSecs)
                frame.extend(line.split(','))
                
                frames.append(frame)

        cnt = cnt + 1
        
    empatica_array = numpy.array(frames).astype(float)
    
    return empatica_array
    
def read_anvilannotationfile(filename):
    
    f = open(filename,'r')
    lines = f.readlines() #python list of strings
    f.close()
    
    frames = []
    
    #time, gesture type, gesture phase, bluffing
    
    for line in lines[1:]:
        
        line_split = re.split(r'\t+', line)
        
        if len(line_split) > 0 :  
            frame = [];
            frame.append(line_split[1])
            
            if line_split[7] == '0':
                frame.extend('-')
            elif line_split[7] == '1':
                frame.extend('P') #place
            elif line_split[7] == '2':
                frame.extend('R') #reveal
            else :
                frame.extend('-')
                
                
            if line_split[10] == '0':
                frame.extend('-')
            elif line_split[10] == '1':
                frame.extend('P') #place
            elif line_split[10] == '2':
                frame.extend('S') #stroke
            elif line_split[10] =='3':
                frame.extend('H') #hold
            elif line_split[10] == '4':
                frame.extend('R') #retract
            else :
                frame.extend('-')
                
                            
            if line_split[8] == '0':
                frame.extend('T') #truth
            elif line_split[8] == '1':
                frame.extend('B')#bluff
            else :
                frame.extend('-')
                
            frames.append(frame)

    empatica_array = numpy.array(frames)
    
    return empatica_array

def get_place_intervals(video_annotation):
    place_intervals = []
    
    place_found = 0
    
    #number, starttime, endtime, output
    interval = []
    cnt = 1
    
    for frame in video_annotation:    
        if frame[1] == 'P':
            if place_found:
                interval[2] = frame[0]
            else:
                place_found = 1
                interval.append ( cnt)
                interval.append ( frame[0])
                interval.append ( frame[0])
                interval.append ( frame[3])
                
        else:
            if place_found:
                place_found = 0
                cnt = cnt + 1
                place_intervals.append(interval)
                interval = []
                
    return place_intervals
    
def get_values_in_time_interval(data_array, column, start_time, end_time):
    
    values = []
    for frame in data_array:
        if (frame[0] > float(end_time)):   
            break
        elif (frame [0] >= float(start_time)):
            values.append(frame[column])
            
    return values
    

#number, starttime, endtime, output, std(acc.x), std(acc.y), std(acc.z), std(temp), std(heartrate), std(skin)
def calculate_inputs (place_intervals, skeleton_array, acc_array, temp_array, heartrate_array, skin_array):
    writer = open(ouput_filename,'a')




    for interval in place_intervals:
        values = get_values_in_time_interval(acc_array, 1, interval[1], interval[2]);
        interval.append(float(numpy.std(values)))
        
        values = get_values_in_time_interval(acc_array,2, interval[1], interval[2]);
        interval.append(float(numpy.std(values)))
        
        values = get_values_in_time_interval(acc_array, 3, interval[1], interval[2]);
        interval.append(float(numpy.std(values)))
        
        values = get_values_in_time_interval(temp_array, 1, interval[1], interval[2]);
        interval.append(float(numpy.std(values)))
        
        values = get_values_in_time_interval(heartrate_array, 1, interval[1], interval[2]);
        interval.append(float(numpy.std(values)))
        
        values = get_values_in_time_interval(skin_array, 1, interval[1], interval[2]);
        interval.append(float(numpy.std(values)))
        
        
        print interval
        writer.write(', '.join(map(str, interval)) + '\r\n') 
        
    writer.close()
    
skeleton_array     = read_skeletonLogger(skeletonLogger_filename)
acc_array = read_empaticafile(accelerometer_filename)
temp_array = read_empaticafile(temperature_filename)
heartrate_array = read_empaticafile(heartrate_filename)
skin_array = read_empaticafile(skinconductance_filename)
video_annotation = read_anvilannotationfile(anvilAnnotation_filename)
place_intervals = get_place_intervals(video_annotation)

calculate_inputs (place_intervals, skeleton_array, acc_array, temp_array, heartrate_array, skin_array)



