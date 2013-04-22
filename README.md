nANT Task for adding a deployment to New Relic
==============================================

A nANT task used to add a new deployment in New Relic. (www.newrelic.com)

Usage
-----
```xml
<loadtasks assembly="NewRelicTask.dll" />
<newrelic license=""
  url=""
  [app_name="" / application_id=""]
/>
```

Available parameters
--------------------
All parameters supported by the New Relic REST API are available:
* license - Your New Relic license *required*
* url - URL to New Relic REST API *required*
* app_name - Name of application that is being deployed *either application_id or app_name is required. Both can not be supplied*
* application_id - Id for the application that is being deployed *either application_id or app_name is required. Both can not be supplied*
* revision - Revision being deployed
* description - Description of the deployment
* changelog - Changelog for the deployment
* user - User that is doing the deployment
* host


