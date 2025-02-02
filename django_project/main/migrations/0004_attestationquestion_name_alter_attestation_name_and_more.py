# Generated by Django 5.0.2 on 2024-05-19 17:40

from django.db import migrations, models


class Migration(migrations.Migration):

    dependencies = [
        ('main', '0003_rename_attestation_question_attestation_attestationquestions'),
    ]

    operations = [
        migrations.AddField(
            model_name='attestationquestion',
            name='name',
            field=models.CharField(default=1, max_length=255),
            preserve_default=False,
        ),
        migrations.AlterField(
            model_name='attestation',
            name='name',
            field=models.CharField(max_length=255),
        ),
        migrations.AlterField(
            model_name='attestationquestion',
            name='right_values',
            field=models.CharField(max_length=255),
        ),
        migrations.AlterField(
            model_name='attestationquestion',
            name='type',
            field=models.CharField(max_length=255),
        ),
        migrations.AlterField(
            model_name='attestationquestion',
            name='values',
            field=models.CharField(blank=True, max_length=255, null=True),
        ),
    ]
