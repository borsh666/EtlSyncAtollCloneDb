select  
       SITE_NAME as SiteName,
       TX_ID as TxId,
       FBAND as Fband,
       ANTENNA_NAME as AntennaName,
       Tilt,
       Azimut,
       Height,
       CF_SITE_PROGRESS as CfSiteProgress,
       CF_PARAMETERS_PLAN as CfParametersPlan
from [dbo].[gtransmitters]
where site_name=@SiteName
union
select 
       SITE_NAME as SiteName,
       TX_ID as TxId,
       FBAND as Fband,
       ANTENNA_NAME as AntennaName,
       Tilt,
       Azimut,
       Height,
       CF_SITE_PROGRESS as CfSiteProgress,
       CF_PARAMETERS_PLAN as CfParametersPlan
from [dbo].[utransmitters]
where site_name=@SiteName
union
select 
       SITE_NAME as SiteName,
       TX_ID as TxId, 
       FBAND as Fband,
       ANTENNA_NAME as AntennaName,
       Tilt,
       Azimut,
       Height,
       CF_SITE_PROGRESS as CfSiteProgress,
       CF_PARAMETERS_PLAN as CfParametersPlan
from [dbo].[xgtransmitters]
where site_name=@SiteName
