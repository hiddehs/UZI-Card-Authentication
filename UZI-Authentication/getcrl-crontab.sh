#!/bin/sh
#
# 1 to 6 Hourly crontab.
#
OUTDIR="."
set -e
(
for url in \
        http://www.uzi-register-test.nl/cdp/test_uzi-register_zorgverlener_ca_g3.crl \
	    http://www.uzi-register-test.nl/cdp/test_zorg_csp_level_2_persoon_ca_g3.crl \
        http://www.uzi-register-test.nl/cdp/test_zorg_csp_root_ca_g3.crl 
do
	curl -s $url | openssl crl -inform DER 
done
) > "$OUTDIR/last.crl.new"
mv  "$OUTDIR/last.crl.new" "$OUTDIR/last.crl"
