

export default function UserMicrocontrollersList() {
  const array = new Array(100);
  array.fill(1, 0, -1)
  
  return (
    <div>
      {array.map(el => (<p>Lorem ipsum dolor sit amet consectetur adipisicing elit. Accusamus minus pariatur eligendi quis, commodi nemo eos magni.
      Consequuntur dolorum, est praesentium, delectus deserunt porro iusto harum magni saepe assumenda accusamus.
      Lorem ipsum dolor sit, amet consectetur adipisicing elit. Reprehenderit illum vitae voluptates sint iusto et commodi, eos rerum, inventore est sunt autem, enim nam odio debitis eveniet minus voluptatem aut!
      Lorem ipsum dolor sit amet consectetur, adipisicing elit. Laboriosam, incidunt numquam. Consectetur deserunt voluptates eius rem nisi error beatae magnam, omnis voluptatum nulla ab corrupti voluptatibus. Maiores, et laborum! Eveniet!
      Lorem ipsum dolor sit amet consectetur adipisicing elit. Ratione ullam quae, maxime reiciendis, quis amet deserunt optio earum sed impedit a soluta sit dolorem totam temporibus. Iure expedita voluptatibus explicabo!</p>))}
    </div>
  );
}