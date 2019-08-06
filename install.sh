#Remove the old folder if it exists
rm -r -f /opt/status-monitor

#Create a clean directory
mkdir -p /opt/status-monitor

#Download the latest version
wget http://jenkins.local:8081/builds/status-monitor/status-monitor-latest.tar.gz

#Extract to the install directory
tar zxf status-monitor-latest.tar.gz -C /opt/status-monitor

#Cleanup the download
rm -f status-monitor-latest.tar.gz