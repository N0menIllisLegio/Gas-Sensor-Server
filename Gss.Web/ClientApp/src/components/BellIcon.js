import { makeStyles, SvgIcon } from "@material-ui/core";

const useStyles = makeStyles((theme) => ({
  bellIcon: {
    marginBottom: theme.spacing(3),
    height: theme.spacing(8),
    width: theme.spacing(8),
  }
}));

export default function BellIcon() {
  const classes = useStyles();

  return (
    <SvgIcon className={classes.bellIcon}>
      <svg id="ic_bell" viewBox="0 0 32 32">
        <path d="m29.187 26.948s-2.2805-3.8013-2.2805-3.8013c-1.0532-1.7545-1.6098-3.7642-1.6098-5.8096v-3.3298c0-4.2038-2.8052-7.7593-6.641-8.9056v-2.3841c0-1.465-1.1914-2.6564-2.6564-2.6564-1.465 0-2.6564 1.1914-2.6564 2.6564v2.3841c-3.8358 1.1463-6.641 4.7018-6.641 8.9056v3.3298c0 2.0454-0.55654 4.0537-1.6085 5.8083l-2.2805 3.8013c-0.12352 0.2059-0.12621 0.46091-0.007969 0.66942 0.11822 0.20852 0.33738 0.33739 0.57645 0.33739h25.236c0.23908 0 0.45953-0.12882 0.57777-0.33602 0.11822-0.20725 0.11424-0.46495-0.007969-0.66947z"></path>
        <path d="m11.817 29.282c0.74911 1.5647 2.335 2.6564 4.1825 2.6564s3.4334-1.0918 4.1825-2.6564h-8.365z"></path>
      </svg>
    </SvgIcon>
  );
}