###--------------------------------------------------------###

   Txt::  Txt Path=E:\
   Xml::  Xml Path=E:\
Access::  Provider=Microsoft.Jet.OLEDB.4.0; Data Source=E:\cyqdata.mdb
Sqlite::  Data Source=E:\cyqdata.db;failifmissing=false;
 MySql::  host=localhost;port=3306;database=cyqdata;uid=root;pwd=123456;Convert Zero Datetime=True;
 Mssql::  server=.;database=cyqdata;uid=sa;pwd=123456; 
Sybase::  data source=127.0.0.1;port=5000;database=cyqdata;uid=sa;pwd=123456;

Oracle OracleClient:: 
Provider=MSDAORA;Data Source=orcl;User ID=sa;Password=123456
Provider=MSDAORA;Data Source=ip\orcl;User ID=sa;Password=123456

Oracle ODP.NET::
Data Source=(DESCRIPTION=(ADDRESS_LIST=(ADDRESS=(PROTOCOL=TCP)(HOST=127.0.0.1)(PORT = 1521)))(CONNECT_DATA =(SID = orcl)));User ID=sa;password=123456

###--------------------------------------------------------###

����˵����
1��{0} �����Ŀ¼
2��sqlite��MySql��Sybase ����Ҫ����Ӧ��dll�ŵ���cyq.data.dllͬһĿ¼��
��ص�dll���أ�http://www.cyqdata.com/download/article-detail-426
3��oracle ʱ��
A��Ĭ��OracleClient������64λ�ģ������32λ�ģ���Ҫ�Լ���Դ���Ƴ���������32λ�ġ�
B����odp.net ��Oracle.DataAccess ��Ҫ�Լ����ذ�װ������Oracle.DataAccess.dll�ŵ���ͬһĿ¼�¡�
C����Oracle.ManagedDataAccess�ŵ�ͬһĿ¼�¼�����ʹ�á�

Explanation:
1: {0} represents the root directory
2: sqlite, MySql, Sybase needs to put the corresponding dll and cyq.data.dll same directory.
Related dll download: http: //www.cyqdata.com/download/article-detail-426
3: oracle when:
A: The default OracleClient lead is 64, if it is 32, you need to remove yourself from the re-introduction of 32-bit source.
B: with odp.net of Oracle.DataAccess need to download to install, and will Oracle.DataAccess.dll into the same directory.
C: with Oracle.ManagedDataAccess into the same directory that is available.