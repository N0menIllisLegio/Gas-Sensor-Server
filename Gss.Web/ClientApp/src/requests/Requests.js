import { authorize } from '../redux/reducers/authSlice';

let dispatch = null;

export function Initialize(_dispatch) {
  dispatch = _dispatch;
}

export async function MakeAuthorizedRequest(requestFactory, user) {
  let response = await requestFactory(user.AccessToken);

  if (response.status === 401) {
    const refreshTokenResponse = await RefreshToken(user.AccessToken, user.RefreshToken);

    if (refreshTokenResponse.errors === null) {
      dispatch(authorize(refreshTokenResponse.data));
      response = await requestFactory(refreshTokenResponse.data.AccessToken);

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

export async function PutRequest(url, body, token) {
  return await Request(url, {
    method: 'PUT',
    headers: {
      'Content-Type': 'application/json',
      'Authorization': `Bearer ${token ?? ''}`
    },
    body: JSON.stringify(body)
  });
}

export async function DeleteRequest(url, token) {
  return await Request(url, {
    method: 'DELETE',
    headers: {
      'Content-Type': 'application/json',
      'Authorization': `Bearer ${token ?? ''}`
    }
  });
}

export async function PostImageRequest(url, image, token) {
  const imageRequestData = new FormData();
  imageRequestData.append('FileForm', image);

  return await Request(url, {
    method: 'POST',
    headers: {
      'Authorization': `Bearer ${token ?? ''}`
    },
    body: imageRequestData
  });
}

export async function Request(url, requestInit) {
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
      errors = response.errors || response.Errors;
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