
# Andrew G. West - 7/29/2009
# Make-file for the CIS400/4001 Project Proposal Specification

	# Filenames to be-used in compilation
MAIN=prop_spec
TEX=prop_spec.tex
DVI=prop_spec.dvi
PS=prop_spec.ps

	# Core compilation commands
CC=latex
BB=bibtex
BUILDPS=dvips -o $(PS) 
BUILDPDF=ps2pdf

all:$(TEX) $(BIB)
	$(CC) $(TEX)
	$(BB) $(MAIN)
	$(CC) $(TEX)
	$(CC) $(TEX)
	$(BUILDPS) $(DVI)
	$(BUILDPDF) $(PS)

clean:
	rm -rf $(DVI) $(PS) *.log *.aux *.bbl *.blg
