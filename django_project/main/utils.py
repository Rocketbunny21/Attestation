from datetime import datetime, timedelta


def date_to_timestamp(date):
    return int(datetime(date.year, date.month, date.day).timestamp())


def date_formatter(date):
    return date.strftime("%d.%m.%Y")


def time_formatter(time):
    return time.strftime("%H:%M")