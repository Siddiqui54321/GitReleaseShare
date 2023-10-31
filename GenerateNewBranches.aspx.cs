using System;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;
using SHMA.Enterprise;
using SHMA.Enterprise.Data;
using shsm;
using SHAB.Data;
using SHMA.Enterprise.Presentation;
using System.Data.SqlClient;
using System.Data.OleDb;

namespace Bancassurance.Presentation
{
    public partial class GenerateNewBranches : System.Web.UI.Page
    {
        public string fixCCSCode = "0";    //1st record 781   - 2nd record 782
        private const string fixBnkCod = "HBL";
        private const string fixImmeSup = "912000";
        private const string Query = "strQrygetaggcode";
        NameValueCollection columnNameValue = new NameValueCollection();
        private string newValue;
        private string ccsauto;


        protected void Page_Load(object sender, EventArgs e)
        {
            string user = System.Convert.ToString(Session["s_USE_USERID"]);
            string userType = ace.Ace_General.getUserType(user);


            if (!IsPostBack)
            {
                if (userType == "A")
                {
                    BindDLLC(); BindDLLCD(); Binddata();
                }
                else
                {
                    lblAlert.ForeColor = System.Drawing.Color.Red;
                    lblAlert.Text = ("You are not Authorized...");
                }


            }
        }

        [Obsolete]
        protected void btnSave_Click(object sender, EventArgs e)
        {

            NameValueCollection columnNameValue = new NameValueCollection();

            columnNameValue.Add("cch_code", ddlCCH_CHANNELCD.SelectedValue.Trim() == "" ? null : ddlCCH_CHANNELCD.SelectedValue); //.Trim().Split('-')[0]);
            columnNameValue.Add("ccd_code", ddlCCD_CHANNELDTLCD.SelectedValue.Trim() == "" ? null : ddlCCD_CHANNELDTLCD.SelectedValue);
            columnNameValue.Add("brCode", txtBranchCode.Text.Trim() == "" ? null : txtBranchCode.Text);
            columnNameValue.Add("brName", txtBranchName.Text.Trim() == "" ? null : txtBranchName.Text);
            string use_password = "bca864d09031109d726859a3fd8458c1";
            string use_userid_t4BSO = "BSO" + ddlCCH_CHANNELCD.Text + ddlCCD_CHANNELDTLCD.Text + txtBranchCode.Text;
            string use_userid_t4BM = "BM" + ddlCCH_CHANNELCD.Text + ddlCCD_CHANNELDTLCD.Text + txtBranchCode.Text;
            columnNameValue.Add("use_userid_t4BSO", use_userid_t4BSO == "" ? null : use_userid_t4BSO);
            columnNameValue.Add("use_userid_t4BM", use_userid_t4BM == "" ? null : use_userid_t4BM);
            columnNameValue.Add("ccs_field1_t2", txtBranchCode.Text.Trim() == "" ? null : txtBranchCode.Text);
            String ccsauto = "";
            string strQry1 = "Select ccs_code from ccs_chanlsubdetl where cch_code = '" + columnNameValue.getObject("cch_code") + "' " +
                "and ccd_code = '" + columnNameValue.getObject("ccd_code") + "' and ccs_code in (select max(a.ccs_code) from ccs_chanlsubdetl a " +
                "where a.cch_code='" + columnNameValue.getObject("cch_code") + "' and a.ccd_code='" + columnNameValue.getObject("ccd_code") + "')";

            rowset rstQry1 = DB.executeQuery(strQry1);
            if (rstQry1.next())
            {
                ccsauto = rstQry1.getString("ccs_code");
            }

            if (int.TryParse(ccsauto, out int ccsauto1))
            {
                // Add 1 to the integer value
                int newValue = ccsauto1 + 1;
                if (newValue > 0)
                {
                    string v = newValue.ToString();
                    int vlen = v.Length;
                    if (vlen == 1)
                    {
                        fixCCSCode = "00" + v;
                    }
                    else if (vlen == 2)
                    {
                        fixCCSCode = "0" + v;
                    }
                    else
                    {
                        fixCCSCode = v;
                    }
                }
                else
                {
                    lblAlert.ForeColor = System.Drawing.Color.Red;
                    lblAlert.Text = "CCS_CoDE Value is null...";// + ex.Message";
                }
            }


            string CSD_TYPE_t7 = ddlCCH_CHANNELCD.Text + ddlCCD_CHANNELDTLCD.Text + fixCCSCode;

            columnNameValue.Add("CSD_TYPE_t7", CSD_TYPE_t7 == "" ? null : CSD_TYPE_t7);

            String logo1 = "";
            string strQry = "select ccd_logo from ccd_channeldetail where cch_code = '" + columnNameValue.getObject("cch_code") + "' and ccd_code = '" + columnNameValue.getObject("ccd_code") + "'";
            rowset rstQry = DB.executeQuery(strQry);
            if (rstQry.next())
            {
                logo1 = rstQry.getString("ccd_logo");
            }
            string logo2 = logo1 + "" + txtBranchCode.Text;
            string ccsDesc = logo1 + "-" + columnNameValue.getObject("brName");

            ///////////////////////////////////use substr			
            String bnkcodeGet = "";
            String strQrybnkcodeGet = "Select distinct decode(pbk_bankcode, 'AKD', '902', 'HBL', '912', 'NBP', '904', 'SBL', '906', 'SILK', '908', 'SMBL', '970', 'UBL', '901', 'AMG', '913', 'BAL', '903', 'BOP', '909', 'FWB', '902', 'NIB', '951', '000') bCode from LAAG_AGENT a where pbk_bankcode = '" + logo1 + "'";
            rowset rstQrybnkcodeGet = DB.executeQuery(strQrybnkcodeGet);
            if (rstQrybnkcodeGet.next())
            {
                bnkcodeGet = rstQrybnkcodeGet.getString("bCode");
            }

            string AAG_IMEDSUPR_12t = bnkcodeGet + "" + "0000";

            columnNameValue.Add("AAG_IMEDSUPR_12t", AAG_IMEDSUPR_12t == "" ? null : AAG_IMEDSUPR_12t);


            String getaggcode = "";
            String strQrygetaggcode = "Select to_char(max(aag_agcode)) vacode from LAAG_AGENT where pbk_bankcode = '" + logo1 + "'";
            rowset rstQrygetaggcode = DB.executeQuery(strQrygetaggcode);
            if (rstQrygetaggcode.next())
            {
                getaggcode = rstQrygetaggcode.getString("vacode");
            }

            string aag_agcode_t4 = bnkcodeGet + "" + txtBranchCode.Text;

            if (ddlCCH_CHANNELCD.SelectedValue != null && ddlCCH_CHANNELCD.SelectedValue != "" && ddlCCD_CHANNELDTLCD.SelectedValue != null && ddlCCD_CHANNELDTLCD.SelectedValue != "" && txtBranchCode.Text != null && txtBranchCode.Text != "" && txtBranchName.Text != null && txtBranchName.Text != "") // && txtAgencyCode.Text != null && txtAgencyCode.Text != "")
            {
                string queryccsField = @"SELECT s.ccs_code, s.cch_code, s.ccd_code  FROM ccs_chanlsubdetl s where ccs_field1 = '" + txtBranchCode.Text + "' and s.cch_code ='" + ddlCCH_CHANNELCD.SelectedValue + "' and s.ccd_code = '" + ddlCCD_CHANNELDTLCD.SelectedValue + "' ";
                DataTable dtf = DB.getDataTable(queryccsField);

                if (dtf.Rows.Count <= 0)
                {

                    string querychk = @"SELECT s.ccs_code, s.cch_code, s.ccd_code  FROM ccs_chanlsubdetl s where s.ccs_code = '" + fixCCSCode + "' and s.cch_code ='" + ddlCCH_CHANNELCD.SelectedValue + "' and s.ccd_code = '" + ddlCCD_CHANNELDTLCD.SelectedValue + "' ";
                    DataTable dt = DB.getDataTable(querychk);
                    if (dt.Rows.Count <= 0)
                    {
                        try
                        {
                            string sqlString1 = "insert into ccs_chanlsubdetl (CCS_CODE,\n" +
                                "CCH_CODE,\n" +
                                "CCD_CODE,\n" +
                                "CCS_DESCR,\n" +
                                "CCS_FIELD1)\n" +
                                "values(\n" +
                                " '" + fixCCSCode + "', \n" +
                                " '" + columnNameValue.getObject("cch_code") + "', \n" +
                                " '" + columnNameValue.getObject("ccd_code") + "', \n" +
                                " '" + ccsDesc + "', \n" +
                                " '" + columnNameValue.getObject("brCode") + "' \n" +
                                ")";
                            DB.executeDML(sqlString1);
                        }
                        catch (Exception)
                        {
                            lblAlert.ForeColor = System.Drawing.Color.Red;
                            lblAlert.Text = "Record Found chSubDel..."; // + ex.Message;
                        }

                        try
                        {
                            string sqlString2a = "insert into LPCH_CHANNEL (ppr_prodcd,\n" +
                                "cch_code,\n" +
                                "ccd_code,\n" +
                                "CCS_CODE)\n" +
                                "values(\n" +
                                " '" + "003" + "', \n" +
                                " '" + columnNameValue.getObject("cch_code") + "', \n" +
                                " '" + columnNameValue.getObject("ccd_code") + "', \n" +
                                " '" + fixCCSCode + "' \n" +
                                ")";
                            DB.executeDML(sqlString2a);


                        }
                        catch (Exception)
                        {
                            lblAlert.ForeColor = System.Drawing.Color.Red;
                            lblAlert.Text = "Record Found in producet-003..."; // + ex.Message;
                        }

                        try
                        {
                            string sqlString2b = "insert into LPCH_CHANNEL (ppr_prodcd,\n" +
                                "cch_code,\n" +
                                "ccd_code,\n" +
                                "CCS_CODE)\n" +
                                "values(\n" +
                                " '" + "005" + "', \n" +
                                " '" + columnNameValue.getObject("cch_code") + "', \n" +
                                " '" + columnNameValue.getObject("ccd_code") + "', \n" +
                                " '" + fixCCSCode + "' \n" +
                                ")";
                            DB.executeDML(sqlString2b);


                        }
                        catch (Exception)
                        {
                            lblAlert.ForeColor = System.Drawing.Color.Red;
                            lblAlert.Text = "Record Found in producet-005..."; // + ex.Message;
                        }

                        try
                        {
                            string sqlString2c = "insert into LPCH_CHANNEL (ppr_prodcd,\n" +
                                "cch_code,\n" +
                                "ccd_code,\n" +
                                "CCS_CODE)\n" +
                                "values(\n" +
                                " '" + "074" + "', \n" +
                                " '" + columnNameValue.getObject("cch_code") + "', \n" +
                                " '" + columnNameValue.getObject("ccd_code") + "', \n" +
                                " '" + fixCCSCode + "' \n" +
                                ")";
                            DB.executeDML(sqlString2c);


                        }
                        catch (Exception)
                        {
                            lblAlert.ForeColor = System.Drawing.Color.Red;
                            lblAlert.Text = "Record Found in producet-074..."; // + ex.Message;
                        }

                        try
                        {
                            string sqlString2 = "insert into use_usermaster (use_userid,\n" +
                                "use_name,\n" +
                                "use_password,\n" +
                                "use_type,\n" +
                                "aag_agcode,\n" +
                                "use_active)\n" +
                                "values(\n" +
                                " '" + columnNameValue.getObject("use_userid_t4BSO") + "', \n" +
                                " '" + columnNameValue.getObject("use_userid_t4BSO") + "', \n" +
                                " '" + use_password + "', \n" +
                                " '" + "S" + "', \n" +
                                " '" + aag_agcode_t4 + "', \n" +
                                " '" + "Y" + "' \n" +
                                ")";
                            DB.executeDML(sqlString2);


                        }
                        catch (Exception)
                        {
                            lblAlert.ForeColor = System.Drawing.Color.Red;
                            lblAlert.Text = "Record Found in BSOusemaster..."; // + ex.Message;
                        }

                        try
                        {
                            string sqlString3 = "insert into use_usermaster (use_userid,\n" +
                                "use_name,\n" +
                                "use_password,\n" +
                                "use_type,\n" +
                                "aag_agcode,\n" +
                                "use_active)\n" +
                                "values(\n" +
                                " '" + columnNameValue.getObject("use_userid_t4BM") + "', \n" +
                                " '" + columnNameValue.getObject("use_userid_t4BM") + "', \n" +
                                " '" + use_password + "', \n" +
                                " '" + "S" + "', \n" +
                                " '" + aag_agcode_t4 + "', \n" +
                                " '" + "Y" + "' \n" +
                                ")";
                            DB.executeDML(sqlString3);
                        }
                        catch (Exception)
                        {
                            lblAlert.ForeColor = System.Drawing.Color.Red;
                            lblAlert.Text = "Record Found in BMusemaster..."; // + ex.Message;
                        }

                        try
                        {
                            string sqlString4 = "insert into LUCN_USERCOUNTRY (USE_USERID,\n" +
                                "CCN_CTRYCD,\n" +
                                "UCN_DEFAULT)\n" +
                                "values(\n" +
                                " '" + columnNameValue.getObject("use_userid_t4BSO") + "', \n" +
                                " '" + "001" + "', \n" +
                                " '" + "Y" + "' \n" +
                                ")";
                            DB.executeDML(sqlString4);

                        }
                        catch (Exception)
                        {
                            lblAlert.ForeColor = System.Drawing.Color.Red;
                            lblAlert.Text = "Record Found in BSOusecountry..."; // + ex.Message;
                        }

                        try
                        {
                            string sqlString5 = "insert into LUCN_USERCOUNTRY (USE_USERID,\n" +
                                "CCN_CTRYCD,\n" +
                                "UCN_DEFAULT)\n" +
                                "values(\n" +
                                " '" + columnNameValue.getObject("use_userid_t4BM") + "', \n" +
                                " '" + "001" + "', \n" +
                                " '" + "Y" + "' \n" +
                                ")";
                            DB.executeDML(sqlString5);
                        }
                        catch (Exception)
                        {
                            lblAlert.ForeColor = System.Drawing.Color.Red;
                            lblAlert.Text = "Record Found in BMusecountry..."; // + ex.Message;
                        }


                        try
                        {

                            string sqlString6 = "insert into luch_userchannel (USE_USERID,\n" +
                                "CCH_CODE,\n" +
                                "UCH_DEFAULT,\n" +
                                "CCD_CODE,\n" +
                                "CCS_CODE)\n" +
                                "values(\n" +
                                " '" + columnNameValue.getObject("use_userid_t4BSO") + "', \n" +
                                " '" + columnNameValue.getObject("cch_code") + "', \n" +
                                " '" + "Y" + "', \n" +
                                " '" + columnNameValue.getObject("ccd_code") + "', \n" +
                                " '" + fixCCSCode + "' \n" +
                                ")";
                            DB.executeDML(sqlString6);
                        }
                        catch (Exception)
                        {
                            lblAlert.ForeColor = System.Drawing.Color.Red;
                            lblAlert.Text = "Record Found in BSOuserchannel..."; // + ex.Message;
                        }

                        try
                        {
                            string sqlString7 = "insert into luch_userchannel (USE_USERID,\n" +
                                "CCH_CODE,\n" +
                                "UCH_DEFAULT,\n" +
                                "CCD_CODE,\n" +
                                "CCS_CODE)\n" +
                                "values(\n" +
                                " '" + columnNameValue.getObject("use_userid_t4BM") + "', \n" +
                                " '" + columnNameValue.getObject("cch_code") + "', \n" +
                                " '" + "Y" + "', \n" +
                                " '" + columnNameValue.getObject("ccd_code") + "', \n" +
                                " '" + fixCCSCode + "' \n" +
                                ")";
                            DB.executeDML(sqlString7);
                        }
                        catch (Exception)
                        {
                            lblAlert.ForeColor = System.Drawing.Color.Red;
                            lblAlert.Text = "Record Found in BMuserchannel..."; // + ex.Message;
                        }

                        try
                        {
                            string sqlString8 = "insert into LCSD_SYSTEMDTL (CSH_ID,\n" +
                                "CSD_TYPE,\n" +
                                "CSD_VALUE)\n" +
                                "values(\n" +
                                " '" + "CHAGT" + "', \n" +
                                " '" + aag_agcode_t4 + "', \n" +
                                " '" + aag_agcode_t4 + "' \n" +
                                ")";
                            DB.executeDML(sqlString8);
                        }
                        catch (Exception)
                        {
                            lblAlert.ForeColor = System.Drawing.Color.Red;
                            lblAlert.Text = "Record Found in LCSD_SYSTEMDTL7..."; // + ex.Message;
                        }

                        try
                        {
                            string sqlString9 = "insert into LCSD_SYSTEMDTL (CSH_ID,\n" +
                                "CSD_TYPE,\n" +
                                "CSD_VALUE)\n" +
                                "values(\n" +
                                " '" + "CHBBR" + "', \n" +
                                " '" + CSD_TYPE_t7 + "', \n" +
                                " '" + columnNameValue.getObject("ccs_field1_t2") + "' \n" +
                                ")";
                            DB.executeDML(sqlString9);
                        }
                        catch (Exception)
                        {
                            lblAlert.ForeColor = System.Drawing.Color.Red;
                            lblAlert.Text = "Record Found in LCSD_SYSTEMDTL8..."; // + ex.Message;
                        }

                        try
                        {
                            string sqlString10 = "insert into LAAG_AGENT (AAG_AGCODE,\n" +
                                "CNT_NATCD,\n" +
                                "PBK_BANKCODE,\n" +
                                "PCM_COMPCODE,\n" +
                                "CHL_LEVEL,\n" +
                                "PCL_LOCATCODE,\n" +
                                "CDG_DESIGCODE,\n" +
                                "CRG_RELGCD,\n" +
                                "AAG_NAME,\n" +
                                "AAG_IMEDSUPR,\n" +
                                "EXT_NACTIVE,\n" +
                                "AAG_DIRECT,\n" +
                                "AAG_BROKER,\n" +
                                "AAG_STATUS,\n" +
                                "AAG_SALARIED)\n" +
                                "values(\n" +
                                " '" + aag_agcode_t4 + "', \n" +
                                " '" + "586" + "', \n" +
                                " '" + logo1 + "', \n" +
                                " '" + "01" + "', \n" +
                                " '" + "001" + "', \n" +
                                " '" + "180" + "', \n" +
                                " '" + "500" + "', \n" +
                                " '" + "999" + "', \n" +
                                " '" + logo1 + "-" + txtBranchName.Text + "', \n" +
                                " '" + columnNameValue.getObject("AAG_IMEDSUPR_12t") + "', \n" +
                                " '" + "Y" + "', \n" +
                                " '" + "N" + "', \n" +
                                " '" + "B" + "', \n" +
                                " '" + "C" + "', \n" +
                                " '" + "N" + "' \n" +
                                ")";

                            DB.executeDML(sqlString10);

                            lblAlert.ForeColor = System.Drawing.Color.Green;
                            lblAlert.Text = "Recoed Saved... ";

                        }
                        catch (Exception)
                        {
                            lblAlert.ForeColor = System.Drawing.Color.Red;
                            lblAlert.Text = "Record Found in SLBANCAPRD.LAAG_AGENT..."; // + ex.Message;
                        }

                        ////////////////////////////////////////////////////////////////////////////
                        ////insert into ILAS Env.
                        ////////////////////////////////////////////////////////////////////////////

                        System.Data.OleDb.OleDbConnection dbCon = null;
                        System.Data.OleDb.OleDbDataAdapter dbAdapter;
                        System.Data.OleDb.OleDbCommand dbCom;
                        try
                        {
                            string sqlString11 = "insert into PBB_BANKBRANCH (PBK_BANKCODE,\n" +
                                "PBB_BRANCHCODE,\n" +
                                "CCN_CTRYCD,\n" +
                                "CCT_CITYCD,\n" +
                                "PBB_BRANCHNAME,\n" +
                                "PBB_DDSTEXTFILE)\n" +
                                "values(\n" +
                                " '" + logo1 + "', \n" +
                                " '" + columnNameValue.getObject("brCode") + "', \n" +
                                " '" + "586" + "', \n" +
                                " '" + "001" + "', \n" +
                                " '" + ccsDesc + "', \n" +
                                " '" + "N" + "' \n" +
                                ")";
                            string str_connString = System.Configuration.ConfigurationSettings.AppSettings["DSNILAS"];
                            dbCon = new System.Data.OleDb.OleDbConnection(str_connString);
                            dbCon.Open();
                            dbAdapter = new System.Data.OleDb.OleDbDataAdapter();
                            dbCom = new System.Data.OleDb.OleDbCommand(sqlString11, dbCon);
                            int x = dbCom.ExecuteNonQuery();
                        }
                        catch (Exception)
                        {
                            lblAlert.ForeColor = System.Drawing.Color.Red;
                            lblAlert.Text = "Record Found in PBB_BANKBRANCH..."; // + ex.Message;
                        }
                        finally
                        {
                            dbCon.Close();
                        }

                        string pcaAccount = "Select distinct decode(pbk_bankcode, 'AKD', '1130202020020020', 'HBL', '1130202020030013', 'NBP', '1130202010050002', 'SBL', '1130202010100001', 'SILK', '1130202010120001', 'SMBL', '1130202010110001', 'UBL', '1130202010060054','BAL', '1013010300500027', 'BOP', '1130202020060001', 'NIB', '1130202010090001', '1111111111111111')  pcaAcc from LAAG_AGENT a where pbk_bankcode = '" + logo1 + "'";
                        rowset rs = DB.executeQuery(pcaAccount);
                        if (rs.next())
                        {
                            pcaAccount = Convert.ToString(rs.getObject("pcaAcc"));
                        }
                        try
                        {
                            string sqlString12 = "insert into PBA_BANKACCOUNT (PBK_BANKCODE,\n" +
                                "PBB_BRANCHCODE,\n" +
                                "PBA_SERIAL,\n" +
                                "PCM_COMPCODE,\n" +
                                "PCA_ACCOUNT,\n" +
                                "PBA_ACTIVE,\n" +
                                "PCL_LOCATCODE)\n" +
                                "values(\n" +
                                " '" + logo1 + "', \n" +
                                " '" + columnNameValue.getObject("brCode") + "', \n" +
                                " '" + "1" + "', \n" +
                                " '" + "01" + "', \n" +
                                " '" + pcaAccount + "', \n" +   //PCA ACCOUNT check it <-------------------------
                                " '" + "1" + "', \n" +
                                " '" + "180" + "' \n" +
                                ")";
                            string str_connString = System.Configuration.ConfigurationSettings.AppSettings["DSNILAS"];
                            dbCon = new System.Data.OleDb.OleDbConnection(str_connString);
                            dbCon.Open();
                            dbAdapter = new System.Data.OleDb.OleDbDataAdapter();
                            dbCom = new System.Data.OleDb.OleDbCommand(sqlString12, dbCon);
                            int x = dbCom.ExecuteNonQuery();
                        }
                        catch (Exception)
                        {
                            lblAlert.ForeColor = System.Drawing.Color.Red;
                            lblAlert.Text = "Record Found in PBA_BANKACCOUNT..."; // + ex.Message;
                        }
                        finally
                        {
                            dbCon.Close();
                        }

                        try
                        {
                            string sqlString13 = "insert into LAAG_AGENT (AAG_AGCODE,\n" +
                                "CNT_NATCD,\n" +
                                "PBK_BANKCODE,\n" +
                                "PCM_COMPCODE,\n" +
                                "CHL_LEVEL,\n" +
                                "PCL_LOCATCODE,\n" +
                                "CDG_DESIGCODE,\n" +
                                "CRG_RELGCD,\n" +
                                "AAG_NAME,\n" +
                                "AAG_IMEDSUPR,\n" +
                                "EXT_NACTIVE,\n" +
                                "AAG_DIRECT,\n" +
                                "AAG_BROKER,\n" +
                                "AAG_STATUS,\n" +
                                "AAG_SALARIED)\n" +
                                "values(\n" +
                                " '" + aag_agcode_t4 + "', \n" +
                                " '" + "586" + "', \n" +
                                " '" + logo1 + "', \n" +
                                " '" + "01" + "', \n" +
                                " '" + "001" + "', \n" +
                                " '" + "180" + "', \n" +
                                " '" + "500" + "', \n" +
                                " '" + "999" + "', \n" +
                                " '" + logo1 + "-" + txtBranchName.Text + "', \n" +
                                " '" + columnNameValue.getObject("AAG_IMEDSUPR_12t") + "', \n" +
                                " '" + "Y" + "', \n" +
                                " '" + "N" + "', \n" +
                                " '" + "B" + "', \n" +
                                " '" + "C" + "', \n" +
                                " '" + "N" + "' \n" +
                                ")";

                            string str_connString = System.Configuration.ConfigurationSettings.AppSettings["DSNILAS"];
                            dbCon = new System.Data.OleDb.OleDbConnection(str_connString);
                            dbCon.Open();
                            dbAdapter = new System.Data.OleDb.OleDbDataAdapter();
                            dbCom = new System.Data.OleDb.OleDbCommand(sqlString13, dbCon);
                            int x = dbCom.ExecuteNonQuery();

                            lblAlert.ForeColor = System.Drawing.Color.Green;
                            lblAlert.Text = "Recoed Saved... ";

                        }
                        catch (Exception)
                        {
                            lblAlert.ForeColor = System.Drawing.Color.Red;
                            lblAlert.Text = "Record Found in ILAS laagAgent..."; // + ex.Message;
                        }
                        finally
                        {
                            dbCon.Close();
                        }




                    }
                    else
                    {
                        lblAlert.ForeColor = System.Drawing.Color.Red;
                        lblAlert.Text = "Record Already Found..."; // + ex.Message;
                    }

                }
                else
                {
                    lblAlert.ForeColor = System.Drawing.Color.Red;
                    lblAlert.Text = "Record Already Found...."; // + ex.Message;
                }

            }
            else
            {
                lblAlert.ForeColor = System.Drawing.Color.Red;
                lblAlert.Text = "Enter Required Fields ...";
            }



        }

        private string SUBSTR(string txtAgencyCode, int v1, int v2)
        {
            throw new NotImplementedException();
        }

        protected void btnSearch_Click(object sender, EventArgs e)
        {
            if (ddlCCH_CHANNELCD.SelectedValue != null && ddlCCH_CHANNELCD.SelectedValue != "" && ddlCCD_CHANNELDTLCD.SelectedValue != null && ddlCCD_CHANNELDTLCD.SelectedValue != "" && txtBranchCode.Text != null && txtBranchCode.Text != "")
            {
                string query = @"SELECT cs.ccs_code, cs.ccs_field1, cs.ccs_descr, cd.ccd_descr FROM ccs_chanlsubdetl cs 
                inner join CCD_CHANNELDETAIL cd on cd.cch_code=cs.cch_code and cd.ccd_code=cs.ccd_code 
                and cs.cch_code ='" + ddlCCH_CHANNELCD.SelectedValue + "' and cs.ccd_code = '" + ddlCCD_CHANNELDTLCD.SelectedValue + "' and cs.ccs_field1 = '" + txtBranchCode.Text + "'";
                DataTable dt = DB.getDataTable(query);
                if (dt.Rows.Count > 0)
                {
                    grdBranchDtl.DataSource = dt;
                    grdBranchDtl.DataBind();
                    txtBranchCode.Text = null;
                    txtBranchName.Text = null;
                    lblAlert.Text = "";
                }
                else
                {
                    lblAlert.ForeColor = System.Drawing.Color.Red;
                    lblAlert.Text = "Record Not Found...";
                    grdBranchDtl.DataSource = null;
                    grdBranchDtl.DataBind();
                    txtBranchCode.Text = null;
                    txtBranchName.Text = null;
                }
            }
            else if (ddlCCH_CHANNELCD.SelectedValue != null && ddlCCH_CHANNELCD.SelectedValue != "" && ddlCCD_CHANNELDTLCD.SelectedValue != null && ddlCCD_CHANNELDTLCD.SelectedValue != "")
            {
                string query = @"SELECT cs.ccs_code, cs.ccs_field1, cs.ccs_descr, cd.ccd_descr FROM ccs_chanlsubdetl cs 
                inner join CCD_CHANNELDETAIL cd on cd.cch_code=cs.cch_code and cd.ccd_code=cs.ccd_code 
                and cs.cch_code ='" + ddlCCH_CHANNELCD.SelectedValue + "' and cs.ccd_code = '" + ddlCCD_CHANNELDTLCD.SelectedValue + "'";
                DataTable dt = DB.getDataTable(query);
                if (dt.Rows.Count > 0)
                {
                    grdBranchDtl.DataSource = dt;
                    grdBranchDtl.DataBind();
                    txtBranchCode.Text = null;
                    txtBranchName.Text = null;
                    lblAlert.Text = "";
                }
                else
                {
                    lblAlert.ForeColor = System.Drawing.Color.Red;
                    lblAlert.Text = "Record Not Found...";
                    grdBranchDtl.DataSource = null;
                    grdBranchDtl.DataBind();
                    txtBranchCode.Text = null;
                    txtBranchName.Text = null;
                }
            }
            else
            {
                lblAlert.ForeColor = System.Drawing.Color.Red;
                lblAlert.Text = "Record Not Found...";
                grdBranchDtl.DataSource = null;
                grdBranchDtl.DataBind();
                txtBranchCode.Text = null;
                txtBranchName.Text = null;
            }
        }

        protected void btnUpdate_Click(object sender, EventArgs e)
        {
            try
            {
                String ccsauto = "";
                string strQry1 = "Select ccs_code from ccs_chanlsubdetl where cch_code = '" + ddlCCH_CHANNELCD.Text + "' and ccd_code = '" + ddlCCD_CHANNELDTLCD.Text + "' and ccs_field1 = '" + txtBranchCode.Text + "'";
                rowset rstQry1 = DB.executeQuery(strQry1);
                if (rstQry1.next())
                {
                    ccsauto = rstQry1.getString("ccs_code");
                }

                SHSM_AuditTrail auditTrail = new SHSM_AuditTrail();
                NameValueCollection columnNameValue = new NameValueCollection();

                string qryUp = "UPDATE ccs_chanlsubdetl SET ccs_descr='" + txtBranchName.Text + "' " +
                " where cch_code = '" + ddlCCH_CHANNELCD.Text + "' and ccd_code = '" + ddlCCD_CHANNELDTLCD.Text + "' and ccs_code = '" + ccsauto + "'";
                DB.executeDML(qryUp);


                String upccdlogo = "";
                String strQrylogo = "select ccd_logo from ccd_channeldetail where cch_code = '" + ddlCCH_CHANNELCD.Text + "' and ccd_code = '" + ddlCCD_CHANNELDTLCD.Text + "'";
                rowset rstQrylogo = DB.executeQuery(strQrylogo);
                if (rstQrylogo.next())
                {
                    //upccdcode = rstQrylogo.getString("ccd_code");
                    upccdlogo = rstQrylogo.getString("ccd_logo");
                }

                String bnkcodeGet = "";
                String strQrybnkcodeGet = "Select distinct decode(pbk_bankcode, 'AKD', '902', 'HBL', '912', 'NBP', '904', 'SBL', '906', 'SILK', '908', 'SMBL', '970', 'UBL', '901', 'AMG', '913', 'BAL', '903', 'BOP', '909', 'FWB', '902', 'NIB', '951', '000') bCode from LAAG_AGENT a where pbk_bankcode = '" + upccdlogo + "'";
                rowset rstQrybnkcodeGet = DB.executeQuery(strQrybnkcodeGet);
                if (rstQrybnkcodeGet.next())
                {
                    bnkcodeGet = rstQrybnkcodeGet.getString("bCode");
                }

                string aag_agcode_t4 = bnkcodeGet + "" + txtBranchCode.Text;

                string qryuplaag = "update laag_agent set aag_name = '" + upccdlogo + "-" + txtBranchName.Text + "' where aag_agcode = '" + aag_agcode_t4 + "'";
                DB.executeDML(qryuplaag);
                lblAlert.ForeColor = System.Drawing.Color.Green;
                lblAlert.Text = "Record Updated ..."; // + ex.Message;

                //UPDATE SLILASPRD 
                System.Data.OleDb.OleDbConnection dbCon = null;
                System.Data.OleDb.OleDbDataAdapter dbAdapter;
                System.Data.OleDb.OleDbCommand dbCom;
                try
                {
                    string qryuplaag1 = "update SLILASPRD.laag_agent set aag_name = '" + upccdlogo + "-" + txtBranchName.Text + "' where aag_agcode = '" + aag_agcode_t4 + "'";
                    string str_connString = System.Configuration.ConfigurationSettings.AppSettings["DSNILAS"];
                    dbCon = new System.Data.OleDb.OleDbConnection(str_connString);
                    dbCon.Open();
                    dbAdapter = new System.Data.OleDb.OleDbDataAdapter();
                    dbCom = new System.Data.OleDb.OleDbCommand(qryuplaag1, dbCon);
                    int x = dbCom.ExecuteNonQuery();
                }
                catch (Exception)
                {
                    lblAlert.ForeColor = System.Drawing.Color.Red;
                    lblAlert.Text = "Record Found in PBB_BANKBRANCH..."; // + ex.Message;
                }
                finally
                {
                    dbCon.Close();
                }



            }
            catch (Exception) //ex
            {
                //throw new Exception(EX.Message);
                lblAlert.ForeColor = System.Drawing.Color.Red;
                lblAlert.Text = "Updated allow only for Branch Name..."; // + ex.Message;
            }
        }

        protected void btnRef_Click(object sender, EventArgs e)
        {
            btnRefersh();
        }

        protected void grdBranchDtl_SelectedIndexChanged(object sender, EventArgs e)
        {
            string txtStaffID_1 = grdBranchDtl.SelectedRow.Cells[0].Text;
            string txtStaffName_1 = grdBranchDtl.SelectedRow.Cells[1].Text;
            string txtStaffName_11 = grdBranchDtl.SelectedRow.Cells[2].Text;
            string txtStaffName_111 = grdBranchDtl.SelectedRow.Cells[3].Text;
            txtBranchCode.Text = txtStaffName_1;
            txtBranchName.Text = txtStaffName_11;
        }

        protected void grdBranchDtl_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                e.Row.Attributes["onclick"] = Page.ClientScript.GetPostBackClientHyperlink(grdBranchDtl, "Select$" + e.Row.RowIndex);
                e.Row.Attributes["style"] = "cursor:pointer";
            }
        }

        protected void BindDLLC()
        {
            IDataReader LCOP_OCCUPATIONReader2 = LCOP_OCCUPATIONDB.GetDDL_ILUS_ET_NM_PER_PERSONALDET_CCH_CHANNEL_RO(); ;
            ddlCCH_CHANNELCD.DataSource = LCOP_OCCUPATIONReader2;
            ddlCCH_CHANNELCD.DataBind();
            LCOP_OCCUPATIONReader2.Close();
        }

        protected void BindDLLCD()
        {
            IDataReader LCOP_OCCUPATIONReader3 = LCOP_OCCUPATIONDB.GetDDL_ILUS_ET_NM_PER_PERSONALDET_CCD_CHANNELDETAIL_RO(); ;
            ddlCCD_CHANNELDTLCD.DataSource = LCOP_OCCUPATIONReader3;
            ddlCCD_CHANNELDTLCD.DataBind();
            LCOP_OCCUPATIONReader3.Close();
        }

        protected void Binddata()
        {

            string query = @"SELECT CS.CCS_CODE, CS.CCS_FIELD1, CS.CCS_DESCR, CD.CCD_DESCR FROM CCS_CHANLSUBDETL CS 
                INNER JOIN CCH_CHANNEL CH ON CH.CCH_CODE = CS.CCH_CODE
                INNER JOIN CCD_CHANNELDETAIL CD ON CD.CCD_CODE = CS.CCD_CODE";

            DataTable dt = DB.getDataTable(query);
            if (dt.Rows.Count > 0)
            {
                grdBranchDtl.DataSource = dt;
                grdBranchDtl.DataBind();
            }
        }

        protected void btnRefersh()
        {
            BindDLLC(); BindDLLCD(); Binddata();
            //txtAgencyCode.Text = "";
            txtBranchCode.Text = "";
            txtBranchName.Text = "";
            lblAlert.Text = "";

        }

    }
}
