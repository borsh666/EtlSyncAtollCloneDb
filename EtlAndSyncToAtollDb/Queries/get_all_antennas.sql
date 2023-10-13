select 
    case 
        when Frequency>925 and Frequency<970 then 900
        when Frequency>1700 and Frequency<1890 then 1800
        when Frequency>1930 and Frequency<2170 then 2100
        when Frequency>2600 and Frequency<2700 then 2600
        when Frequency>3500 and Frequency<3600 then 3600
    end as Band,
    name as Pattern,
    ELECTRICAL_TILT as Etilt,
    PHYSICAL_ANTENNA as AntennaName,
    Frequency
from antennas
