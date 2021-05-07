import { authorize } from '../redux/reducers/authSlice';

export async function MakeAuthorizedRequest(requestFactory, dispatch, accessToken, refreshToken) {
  let response = await requestFactory();
  console.log(accessToken);

  if (response.status === 401 || true) {
    const refreshTokenResponse = await RefreshToken(accessToken, refreshToken);

    if (refreshTokenResponse.errors === null) {
      dispatch(authorize(refreshTokenResponse.data));
      response = await requestFactory();

    } else {
      return refreshTokenResponse;
    }
  }

  return response;
}

export async function PostRequest(url, body, token) {
  return await Request(url, {
    method: 'POST',
    headers: {
      'Content-Type': 'application/json',
      'Authorization': `Bearer ${token ?? ''}`
    },
    body: JSON.stringify(body)
  });
}

export async function GetRequest(url, token) {
  return await Request(url, {
    method: 'GET',
    headers: {
      'Content-Type': 'application/json',
      'Authorization': `Bearer ${token ?? ''}`
    }
  });
}

async function Request(url, requestInit) {
  let data = null;
  let errors = null;
  let status = null;

  try {
    let response = await fetch(url, requestInit);

    status = response.status;
    response = await response.json();

    if (response.Succeeded) {
      data = response.Data;
    } else {
      errors = response.errors;
    }
  } catch (e) {
    errors = [e.Name];
  }

  return { data, errors, status };
}

async function RefreshToken(accessToken, refreshToken) {
  return await PostRequest(`${process.env.REACT_APP_SERVER_URL}api/Authorization/RefreshToken`, {
    'AccessToken': accessToken,
    'RefreshToken': refreshToken
  });
}