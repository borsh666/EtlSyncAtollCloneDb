    select * from 
    (
        select  
            SITE_NAME,
            substring(TX_ID,5,1) as Sector,
            TX_ID,
            AZIMUT,
            Fband,
            ANTENNA_NAME,
            HEIGHT,
            TILT,
            POWER,
            CF_SITE_PROGRESS,
            CF_PARAMETERS_PLAN
        from ATOLL_5GMRAT.dbo.gtransmitters
        where SITE_NAME = @SiteName
        
        union
        
        select 
            SITE_NAME,
            substring(TX_ID,5,1) as Sector,
            TX_ID,
            AZIMUT,
            Fband,
            ANTENNA_NAME,
            HEIGHT,
            TILT,
            TX_MAX_POWER as POWER,
            CF_SITE_PROGRESS,
            CF_PARAMETERS_PLAN
        from ATOLL_5GMRAT.dbo.utransmitters
        where SITE_NAME = @SiteName
        
        union
        
        select  
            SITE_NAME,
            substring(TX_ID,5,1) as Sector,
            TX_ID,
            AZIMUT,
            Fband,
            ANTENNA_NAME,
            HEIGHT,
            TILT,
            '' as POWER,
            CF_SITE_PROGRESS,
            CF_PARAMETERS_PLAN
        from ATOLL_5GMRAT.dbo.xgtransmitters
        where SITE_NAME = @SiteName
    ) b


  

except

  select * from 
    (
        select  
            SITE_NAME,
            substring(TX_ID,5,1) as Sector,
            TX_ID,
            AZIMUT,
            Fband,
            ANTENNA_NAME,
            HEIGHT,
            TILT,
            POWER,
            CF_SITE_PROGRESS,
            CF_PARAMETERS_PLAN
        from gtransmitters
        where SITE_NAME = @SiteName
        
        union
        
        select 
            SITE_NAME,
            substring(TX_ID,5,1) as Sector,
            TX_ID,
            AZIMUT,
            Fband,
            ANTENNA_NAME,
            HEIGHT,
            TILT,
            TX_MAX_POWER as POWER,
            CF_SITE_PROGRESS,
            CF_PARAMETERS_PLAN
        from utransmitters
        where SITE_NAME = @SiteName
        
        union
        
        select  
            SITE_NAME,
            substring(TX_ID,5,1) as Sector,
            TX_ID,
            AZIMUT,
            Fband,
            ANTENNA_NAME,
            HEIGHT,
            TILT,
            '' as POWER,
            CF_SITE_PROGRESS,
            CF_PARAMETERS_PLAN
            from xgtransmitters
        where SITE_NAME = @SiteName
    ) a
