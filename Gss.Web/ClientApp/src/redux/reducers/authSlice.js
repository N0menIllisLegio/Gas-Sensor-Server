import { createSlice } from '@reduxjs/toolkit';

const localStorageKey = 'CurrentUser';
const buildUser = (data) => {
  const { AccessToken, AccessTokenExpiration, RefreshToken, UserID, Administrator } = data;

  if (UserID != null && AccessToken != null && AccessTokenExpiration != null
    && RefreshToken != null && Administrator != null) {
    return {
      UserID,
      AccessToken,
      AccessTokenExpiration,
      RefreshToken,
      Administrator
    };
  }

  return null;
};

export const authSlice = createSlice({
  name: 'auth',
  initialState: {
    user: null
  },
  reducers: {
    initialize: state => {
      let storageItem = localStorage.getItem(localStorageKey);
    
      try {
        let parsedStorageItem = JSON.parse(storageItem);
        const user = buildUser(parsedStorageItem);
        console.log(user);
        state.user = user;
      } catch { }
    },
    authorize: (state, action) => {
      const user = buildUser(action.payload);

      if (user != null) {
        state.user = user;
        localStorage.setItem(localStorageKey, JSON.stringify(user));
      }
    },
    logout: state => {
      state.user = null;
      localStorage.removeItem(localStorageKey);
    }
  }
});

export const { initialize, authorize, logout } = authSlice.actions;
export const selectUser = state => state.authenticator.user;

export default authSlice.reducer;
