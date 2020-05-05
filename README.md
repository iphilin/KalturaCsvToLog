# KalturaCsvToLog

> There is a critical issue, see code for more info! Kibana wraps messages into \" and escape any \" inside messages. We are looking for string messages on position 2 and it works. For other situation it won't.

## Usage

Put KalturaCsvToLog.exe in folder with CSV files (imported logs from kibana) and run. 

Default arguments:

- Column name - `message`
- CSV Separator - `,`