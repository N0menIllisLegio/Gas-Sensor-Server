namespace Gss.MicrocontrollerListener;

record ReadSensorsData(Guid MicrocontrollerSensorId, DateTime ReadTime, double Value);