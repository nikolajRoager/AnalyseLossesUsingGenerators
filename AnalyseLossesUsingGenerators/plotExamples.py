import matplotlib.pyplot as plt
import numpy as np




#This is a simple python script for plotting my example data, it can not handle custom data

def getCSVData(filename):
    with open(filename,'r') as csvfile:#Open in reading mode, no need to worry about closing
        headers =csvfile.readline().split(',')
        data  =csvfile.readline().split(',')

        newData = []
        for h in data[:(len(data)-1)]:
            newData.append(float(h))


        return headers[:(len(data)-1)],newData


def plotPieChart(filename, title):
    try:
        headers,data= getCSVData(filename)
    except Exception as e:
        print("Could not read data, exception")
        print(str(e))
        exit

    #Remove any data which does not contribute
    for i in reversed(range(0,len(data))):
        if (data[i]==0):
            del data[i]
            del headers[i]

    fig, ax = plt.subplots()

    #matplotlib is uggly, Here i try to plot the labels out of the way
    wedges, texts, autotexts= ax.pie(data, labels=headers, autopct='%1.2f%%', startangle=140,labeldistance=1.5,radius=0.8)

    #Last angle where we did plot the label
    last_true_angle=-1
    # Beautify the plot
    for i, p in enumerate(wedges):
        ang = (p.theta2 - p.theta1)/2. + p.theta1

        y0 = np.sin(np.deg2rad(ang))*0.8
        x0 = np.cos(np.deg2rad(ang))*0.8
        if (ang-last_true_angle<4):
            ang = last_true_angle +4 

        last_true_angle=ang
        y = np.sin(np.deg2rad(ang))*1.2
        x = np.cos(np.deg2rad(ang))*1.2
        a=ax.arrow(x,y,x0-x,y0-y)
        a.set_color(p.get_facecolor())
        texts[i].set_x(x)
        texts[i].set_fontsize(8)
        texts[i].set_y(y)
        texts[i].set_rotation(ang)
        autotexts[i].set_alpha(0)
        texts[i].set_alpha(0)
        ax.annotate(texts[i].get_text()+" "+autotexts[i].get_text(),xy=(x,y),xytext=(x*1.15,y*1.15),rotation=ang,rotation_mode='anchor',ha='center',va='center',fontsize=8)
        



    ax.set_title(title);
    ax.set_aspect('equal')
    plt.show()

def plotPieChartG(filename, title, groups, groupnames):
    try:
        headers,data= getCSVData(filename)
    except Exception as e:
        print("Could not read data, exception")
        print(str(e))
        exit

    fig, ax = plt.subplots()

    #Sum up how many elements are in each group
    RoleData = [];
    start=0
    for i in groups:
        sum = 0.0;
        for id in range(start,start+i):
            sum+=data[id]
        RoleData.append(sum)
        start=start+i;
        

    #Remove any data which does not contribute (now after summing groups)
    for i in reversed(range(0,len(data))):
        if (data[i]==0):
            del data[i]
            del headers[i]


    #Print outer plot
    wedges, texts= ax.pie(RoleData, radius=1.2, startangle=90, wedgeprops={'linewidth': 1, 'edgecolor': 'white'},labels=groupnames,labeldistance=0.6)
    for i,p in enumerate(wedges):
        ang = (p.theta2 - p.theta1)/2. + p.theta1
        texts[i].set_rotation(ang-90);


    #Print INNER plot
    wedgesI, textsI= ax.pie(data, radius=0.80, startangle=90, wedgeprops={'linewidth': 1, 'edgecolor': 'white'},labels=headers,labeldistance=0.5)

    for i,p in enumerate(wedgesI):
        ang = (p.theta2 - p.theta1)/2. + p.theta1
        y = np.sin(np.deg2rad(ang))*0.5
        x = np.cos(np.deg2rad(ang))*0.5

        ax.annotate(textsI[i].get_text(),xy=(x,y),xytext=(x,y),rotation=ang,rotation_mode='anchor',ha='center',va='center',fontsize=8)
        textsI[i].set_alpha(0)

    # Adjust the aspect ratio to ensure the pie is circular
    ax.set_aspect('equal')

    ax.set_title(title);
    plt.show()

def plotTwoBars(filenameBlue,filenameRed,blueForce,redForce,title):
    try:
        headers,bdata= getCSVData(filenameBlue)
        headers,rdata= getCSVData(filenameRed)
    except Exception as e:
        print("Could not read data, exception")
        print(str(e))
        exit

    fig, ax = plt.subplots()

    ax.barh(headers,-np.array(bdata),color='blue',label=blueForce)
    ax.barh(headers,rdata,color='red',label=redForce)
    ax.set_title(title)
    ax.legend(loc='lower center', bbox_to_anchor=(0.5, 1.04),ncol=4)
    plt.show()


plotPieChartG(".\\bin\\debug\\net8.0\\Russian_artillery_and_rockets.csv", "Russian Artillery losses",[4,3], ["Ground Support","Anti Air"]);
plotPieChartG(".\\bin\\Debug\\net8.0\\Ukrainian_Artillery_and_rockets.csv", "Ukrainian Artillery losses",[4,3], ["Ground Support","Anti Air"]);


plotTwoBars(".\\bin\\Debug\\net8.0\\ukr_cal_Artillery.csv",".\\bin\\Debug\\net8.0\\rus_cal_Artillery.csv", "Ukraine", "Russia", "Tube artillery lost");


plotTwoBars(".\\bin\\Debug\\net8.0\\ukr_tanks.csv",".\\bin\\Debug\\net8.0\\rus_tanks.csv", "Ukraine", "Russia", "Tanks lost, by generation");


plotPieChart(".\\bin\\Debug\\net8.0\\ukr_nations.csv", "");
plotPieChart(".\\bin\\Debug\\net8.0\\rus_nations.csv", "");

plotTwoBars(".\\bin\\Debug\\net8.0\\ukr_air.csv",".\\bin\\Debug\\net8.0\\rus_air.csv", "Ukraine", "Russia", "Aircrafts lost, by roll");
plotTwoBars(".\\bin\\Debug\\net8.0\\ukr_air_type.csv",".\\bin\\Debug\\net8.0\\rus_air_type.csv", "Ukraine", "Russia", "Aircrafts lost, by type");



plotTwoBars(".\\bin\\Debug\\net8.0\\ukr_vehicle.csv",".\\bin\\Debug\\net8.0\\rus_vehicle.csv", "Ukraine", "Russia", "Vehicles lost, by class");