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
    user: null,
    userBadgeName: null,
    userBadgeAvatarSrc: null,
  },
  reducers: {
    initialize: state => {
      let storageItem = localStorage.getItem(localStorageKey);
    
      try {
        let parsedStorageItem = JSON.parse(storageItem);
        const user = buildUser(parsedStorageItem);
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
      state.userBadgeName = null;
      state.userBadgeAvatarSrc = null;
      localStorage.removeItem(localStorageKey);
    },
    saveBadgeData: (state, action) => {
      state.userBadgeName = action.payload.userBadgeName;
      state.userBadgeAvatarSrc = action.payload.userBadgeAvatarSrc?.replace('/images/','/thumbnails/');
    }
  }
});

export const { initialize, authorize, logout, saveBadgeData } = authSlice.actions;
export const selectUser = state => state.authenticator.user;
export const selectUserBadgeName = state => state.authenticator.userBadgeName;
export const selectUserBadgeAvatarSrc = state => state.authenticator.userBadgeAvatarSrc;

export default authSlice.reducer;
