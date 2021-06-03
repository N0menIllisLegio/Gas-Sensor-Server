import Button from '@material-ui/core/Button';
import { Avatar, Typography } from '@material-ui/core';
import { useState } from 'react';

export default function AvatarButton(props) {
  const [imageName, setImageName] = useState('');

  const handleImageChange = (e) => {
    const input = e.target;
        
    if (input.files && input.files[0]) {
      props.setImage(input.files[0]);
      setImageName(input.files[0].name);

      const fileReader = new FileReader();
      fileReader.onload = (e) => props.setImageSrc(e.target.result);
      fileReader.readAsDataURL(input.files[0]);
    }
  };

  return (
    <div style={{display: 'flex', flexDirection: 'column', alignItems: 'center'}}>
      <input
        accept="image/png, image/jpg, image/jpeg, image/gif"
        type="file" id="image-file-input"
        onChange={(e) => handleImageChange(e)}
        hidden />

      <label htmlFor="image-file-input">
        <Button component="span">
          <Avatar
            src={props.imageSrc}
            variant="rounded"
            style={{
              width: props.avatarWidth,
              height: props.avatarHeight
            }}>{props.placeholderText}</Avatar>
        </Button>
      </label>
      <div style={{overflow: 'hidden', display: 'flex', width: props.avatarWidth}}>
        <Typography variant="caption" style={{textOverflow: 'ellipsis', whiteSpace: 'nowrap', overflow: 'hidden' }}>
          {imageName}
        </Typography>
      </div>
    </div>
  );
}