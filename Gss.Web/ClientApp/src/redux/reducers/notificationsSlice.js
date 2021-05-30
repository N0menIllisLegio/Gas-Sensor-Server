import { createSlice } from '@reduxjs/toolkit';

export const notificationsSlice = createSlice({
  name: 'notifications',
  initialState: {
    receivedNotifications: [],
  },
  reducers: {
    addNotification: (state, action) => {
      const newNotification = action.payload;

      if ('microcontrollerID' in newNotification
        && 'sensorID' in newNotification
        && 'sensorName' in newNotification
        && 'sensorType' in newNotification
        && 'sensorValue' in newNotification
        && 'sensorTypeUnits' in newNotification) {
          let id = 1;
          newNotification.isOld = false;
          const newNotificationsArray = [...state.receivedNotifications, newNotification];
          newNotificationsArray.forEach((notification) => {
            notification.ID = id++;
          });

          state.receivedNotifications = newNotificationsArray;
      }
    },
    removeNotification: (state, action) => {
      const removingNotificationID =  action.payload;
      state.receivedNotifications = state.receivedNotifications
        .filter(notification => notification.ID !== removingNotificationID);
    },
    markMicrocontrollerNotificationsAsOld: (state, action) => {
      const { microcontrollerID, sensorID } =  action.payload;

      const receivedNotifications = [...state.receivedNotifications];
      receivedNotifications.forEach((notification) => {
        if (notification.microcontrollerID === microcontrollerID
          && notification.sensorID === sensorID) {
          notification.isOld = true;
        }
      });

      state.receivedNotifications = receivedNotifications;
    },
  }
});

export const { addNotification, removeNotification, markMicrocontrollerNotificationsAsOld } = notificationsSlice.actions;
export const selectNotifications = state => state.notificator.receivedNotifications;

export default notificationsSlice.reducer;
