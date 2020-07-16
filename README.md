# github-action-releasemail

Provides a GitHub-Action thats sends an email, if a Release is created. It uses sendgrid with a defined template.

## Sendgrid Template-Data

```json
{
    "release" :
    {
        "name" : "releasename",
        "tag" : "releasetagname",
        "body" : "releasebody",
        "link" : "linkToRelease"
    },
    "autor":
    {
        "avatarurl" : "linkToAvatar",
        "name": "autorName"
    }
}

```

## Workflow-Sample

```yml
name: E-Mail Release Notification
on:
  release:
    types: [published]
jobs:
  notify:
    runs-on: ubuntu-latest
    steps:
    - name: Notify about a new release
      uses: ckrowiorsch/action-release-mail@v1.0.0
      env:
        SENDGRID_APIKEY: ${{ secrets.SENDGRID_APIKEY }}
        SENDGRID_TEMPLATE_ID: ${{ secrets.SENDGRID_TEMPLATEID }}
        SENDER_MAIL: <Sender>
        RECEIPIENTS: <receipient-commaseparated>
```
