import { configureStore } from '@reduxjs/toolkit';
import authReducer from './reducers/authSlice';
import notificationsReducer from './reducers/notificationsSlice';

export default configureStore({
  reducer: {
    authenticator: authReducer,
    notificator: notificationsReducer
  }
});